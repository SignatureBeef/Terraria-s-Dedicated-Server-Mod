using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using System.Security.Cryptography;
using System.Text;

using Terraria_Server.Misc;
using Terraria_Server.Logging;
using Terraria_Server.Commands;
using Terraria_Server.Language;

namespace Terraria_Server.RemoteConsole
{
	public static class RConServer
	{
		static volatile bool exit = false;
		static ProgramThread thread;
		static List<RConClient> clients = new List<RConClient>();
		internal static Queue<RConClient> deadClients = new Queue<RConClient>();
		static TcpListener listener;

		public static PropertiesFile LoginDatabase { get; private set; }

		public static void Start(string dbPath)
		{
			LoginDatabase = new PropertiesFile(dbPath);
			LoginDatabase.Load();

			if (LoginDatabase.Count == 0)
			{
				var bytes = new byte[8];
				(new Random((int)DateTime.Now.Ticks)).NextBytes(bytes);

				string password = String.Format("{0:x2}{1:x2}-{2:x2}{3:x2}-{4:x2}{5:x2}-{6:x2}{7:x2}",
					bytes[0], bytes[1], bytes[2], bytes[3], bytes[4], bytes[5], bytes[6], bytes[7]);
				string login = "Owner";
				ProgramLog.Admin.Log("The rcon login database was empty, a new user \"{1}\" has been created with password: {0}", password, login);

				LoginDatabase.setValue(login, Hash(login, password));
			}

			LoginDatabase.Save();

			var bind = Program.properties.RConBindAddress;
			var split = bind.Split(':');
			IPAddress addr;
			ushort port;

			if (split.Length != 2 || !IPAddress.TryParse(split[0], out addr) || !ushort.TryParse(split[1], out port) || port < 1)
			{
				ProgramLog.Error.Log("{0} is not a valid bind address, remote console disabled.", bind);
				return;
			}

			listener = new TcpListener(addr, port);

			try
			{
				listener.Start();
			}
			catch (Exception)
			{
				ProgramLog.Error.Log("Failed to bind to address {0}, remote console disabled.", bind);
				//ProgramLog.Log (exception, "Failed to bind to address 127.0.0.1:" + 7776);
				return;
			}

			thread = new ProgramThread("RCon", RConLoop);
			thread.Start();
		}

		public static void Stop()
		{
			exit = true;
		}

		internal static string Hash(string username, string password)
		{
			var hash = SHA256.Create();
			var sb = new StringBuilder(64);
			var bytes = hash.ComputeHash(Encoding.ASCII.GetBytes(string.Concat(username, ":", Program.properties.RConHashNonce, ":", password)));
			foreach (var b in bytes)
				sb.Append(b.ToString("x2"));
			return sb.ToString();
		}

		public static void RConLoop()
		{
			ProgramLog.Admin.Log("Remote console server started on {0}.", Program.properties.RConBindAddress);

			var socketToObject = new Dictionary<Socket, RConClient>();
			var readList = new List<Socket>();
			var errorList = new List<Socket>();
			var clientList = new List<Socket>();
			var serverSock = listener.Server;

			try //TODO: need to get this socket closing code in order
			{
				while (!exit)
				{
					readList.Clear();
					readList.Add(serverSock);
					readList.AddRange(clientList);
					errorList.Clear();
					errorList.AddRange(clientList);

					try
					{
						Socket.Select(readList, null, errorList, 500000);
					}
					catch (SocketException e)
					{
						ProgramLog.Log(e, "Remote console server loop select exception");

						// sigh, so many places to handle errors

						listener.Pending();

						var newList = new List<Socket>();
						foreach (var sock in clientList)
						{
							var close = false;
							try
							{
								if (sock.Connected) newList.Add(sock);
								else close = true;
							}
							catch
							{
								close = true;
							}

							if (close && socketToObject.ContainsKey(sock))
								DisposeClient(socketToObject[sock]);
						}

						clientList = newList;
						continue;
					}

					lock (deadClients)
						while (deadClients.Count > 0)
						{
							var rcon = deadClients.Dequeue();
							errorList.Remove(rcon.socket);
							clientList.Remove(rcon.socket);
							readList.Remove(rcon.socket);
							DisposeClient(rcon);
						}

					foreach (var sock in errorList)
					{
						CheckError(sock, socketToObject);

						if (socketToObject.ContainsKey(sock))
							DisposeClient(socketToObject[sock]);

						clientList.Remove(sock);
						readList.Remove(sock);
					}

					if (exit) break;

					foreach (var sock in readList)
					{
						if (sock == serverSock)
						{
							// Accept new clients
							while (listener.Pending())
							{
								var client = listener.AcceptSocket();
								var rcon = AcceptClient(client);
								if (rcon != null)
								{
									clientList.Add(client);
									socketToObject[client] = rcon;
								}
							}
						}
						else
						{
							// Handle existing clients
							bool rem = false;
							RConClient rcon = null;
							try
							{
								rcon = socketToObject[sock];
								rem = !ReadFromClient(rcon, sock);
							}
							catch (Exception e)
							{
								HandleSocketException(e);
								rem = true;
							}

							if (rem)
							{
								sock.SafeClose();

								if (rcon != null)
								{
									DisposeClient(rcon);
								}

								clientList.Remove(sock);
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				ProgramLog.Log(e, "RConLoop terminated with exception");
			}

			try
			{
				listener.Stop();
			}
			catch (SocketException) { }

			ProgramLog.Admin.Log("Remote console server stopped.");
		}

		static RConClient AcceptClient(Socket client)
		{
			client.NoDelay = false;

			string addr;
			try
			{
				var rep = client.RemoteEndPoint;
				if (rep != null)
					addr = rep.ToString();
				else
				{
					ProgramLog.Admin.Log("Accepted socket disconnected");
					return null;
				}

				ProgramLog.Admin.Log("New remote console connection from: {0}", addr);

				var rcon = new RConClient(client, addr);

				rcon.Greet();

				clients.Add(rcon);

				return rcon;
			}
			catch (Exception e)
			{
				ProgramLog.Error.Log("Accepted socket exception ({0})", HandleSocketException(e));
				return null;
			}
		}

		static bool ReadFromClient(RConClient rcon, Socket socket)
		{
			var buf = rcon.readBuffer;
			int recv = -1;

			try
			{
				recv =
					socket.Receive(
						buf,
						rcon.bytesRead,
						buf.Length - rcon.bytesRead,
						0);
			}
			catch (Exception e)
			{
				ProgramLog.Debug.Log("{0}: socket exception ({1})", rcon.Id, HandleSocketException(e));
				return false;
			}

			if (recv > 0)
			{
				try
				{
					rcon.bytesRead += recv;
					return rcon.ProcessRead();
				}
				catch (Exception e)
				{
					var msg = HandleSocketException(e, false);
					if (msg == null)
						ProgramLog.Log(e, "Error processing remote console data stream");
					else
						ProgramLog.Debug.Log("{0}: socket exception ({1})", rcon.Id, msg);
				}
			}

			return false;
		}

		static byte[] errorBuf = new byte[1];
		static void CheckError(Socket socket, Dictionary<Socket, RConClient> socketToObject)
		{
			string addr = "<address lost>";
			RConClient rcon = null;

			if (socketToObject.ContainsKey(socket))
			{
				rcon = socketToObject[socket];
				addr = rcon.remoteAddress;
			}

			try
			{
				addr = socket.RemoteEndPoint.ToString();
			}
			catch (Exception) { }

			try
			{
				socket.Receive(errorBuf);
				ProgramLog.Admin.Log("{0}: remote console connection closed", rcon.Id);
			}
			catch (Exception e)
			{
				HandleSocketException(e);
				ProgramLog.Admin.Log("{0}: remote console connection closed", rcon.Id);
			}

			socket.SafeClose();
		}

		static void DisposeClient(RConClient rcon)
		{
			ProgramLog.Admin.Log("{0}: remote console connection closed.", rcon.Id);
			clients.Remove(rcon);
			rcon.Close();
		}

		static string HandleSocketException(Exception e, bool thrownew = true)
		{
			if (e is SocketException)
				return e.Message;
			if (e is System.IO.IOException)
				return e.Message;
			else if (e is ObjectDisposedException)
				return "Socket already disposed";
			else if (thrownew)
				throw new Exception("Unexpected exception in socket handling code", e);
			else
				return null;
		}

		internal static void RConCommand(ISender sender, ArgumentList args)
		{
			string name, pass;
			if (args.TryPop("add") && args.TryParseTwo(out name, out pass))
			{
				if (sender is Player || sender is RConSender)
				{
					sender.sendMessage(Languages.PermissionsError, 255, 238, 130, 238);
					return;
				}

				var password = Hash(name, pass);
				LoginDatabase.setValue(name, password);
				LoginDatabase.Save(false);
				ProgramLog.Log("User `{0}` was added to the RCON Database.", name);
			}
			else if (args.TryParseOne("cut", out name))
			{
				if (sender is Player || sender is RConSender)
				{
					sender.sendMessage(Languages.PermissionsError, 255, 238, 130, 238);
					return;
				}

				var lower = name.ToLower();
				foreach (var rcon in clients)
				{
					if (rcon.Name.ToLower() == lower)
						rcon.Close();
				}

				ProgramLog.Admin.Log("Cut all remote console connections from {0}.", name);
			}
			else if (args.TryParseOne("ban", out name))
			{
				if (sender is Player || sender is RConSender)
				{
					sender.sendMessage(Languages.PermissionsError, 255, 238, 130, 238);
					return;
				}

				LoginDatabase.setValue(name, null);

				var lower = name.ToLower();
				foreach (var rcon in clients)
				{
					if (rcon.Name.ToLower() == lower)
						rcon.Close();
				}

				ProgramLog.Admin.Log("Cut all remote console connections from {0} and revoked credentials.", name);
			}
			else if (args.Count == 1 && args.GetString(0) == "list")
			{
				foreach (var rcon in clients)
				{
					sender.sendMessage(String.Format("{0} {1}", rcon.Id, rcon.state));
				}
			}
			else if (args.Count == 1 && args.GetString(0) == "load")
			{
				if (sender is Player || sender is RConSender)
				{
					sender.sendMessage(Languages.PermissionsError, 255, 238, 130, 238);
					return;
				}

				LoginDatabase.Load();
				ProgramLog.Admin.Log("Reloaded remote console login database.");
			}
			else
			{
				throw new CommandError("");
			}
		}
	}
}

