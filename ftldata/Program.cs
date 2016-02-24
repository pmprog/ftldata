using System;

namespace ftldata
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			ResourceDat datafile;

			Console.WriteLine("FTL Data Packer");
			Console.WriteLine("===============");
			Console.WriteLine();

			if (args.Length < 2)
			{
				WriteUsage();
			}
			else
			{
				switch( args[1] )
				{
					case "list":
						datafile = new ResourceDat(args[0]);
						foreach( FileDescriptor fd in datafile.Files)
						{
							Console.WriteLine(fd.Filename);
						}
						break;

					case "extract":
						if (args.Length < 3)
						{
							WriteUsage();
						}
						else
						{
							datafile = new ResourceDat(args[0]);
							foreach (FileDescriptor fd in datafile.Files)
							{
								if( fd.Length >= 0 && fd.Filename.EndsWith(args[2]))
								{
									string filename = fd.Filename.Replace('/', System.IO.Path.DirectorySeparatorChar);
									string filedir = System.IO.Path.GetDirectoryName(filename);
									if (!System.IO.Directory.Exists(filedir))
									{
										System.IO.Directory.CreateDirectory(filedir);
									}
									Console.WriteLine("Extracting " + fd.Filename + " to " + filename);
									System.IO.File.WriteAllBytes(filename, fd.Data);
								}
							}
						}
						break;

					case "extractall":
						datafile = new ResourceDat(args[0]);
						foreach( FileDescriptor fd in datafile.Files)
						{
							if (fd.Length >= 0)
							{
								Console.WriteLine(fd.Filename);
								string filename = fd.Filename.Replace('/', System.IO.Path.DirectorySeparatorChar);
								string filedir = System.IO.Path.GetDirectoryName(filename);
								if (!System.IO.Directory.Exists(filedir))
								{
									System.IO.Directory.CreateDirectory(filedir);
								}
								Console.WriteLine("Extracting " + fd.Filename + " to " + filename);
								System.IO.File.WriteAllBytes(filename, fd.Data);
							}
						}
						break;

					case "replace":
						datafile = new ResourceDat(args[0]);

						if (args.Length < 4)
						{
							WriteUsage();
						}
						else
						{
							bool foundfile = false;
							foreach (FileDescriptor fd in datafile.Files)
							{
								if (fd.Length >= 0 && fd.Filename == args[2])
								{
									foundfile = true;
									Console.WriteLine("Found file " + fd.Filename + " (Size " + fd.Length.ToString() + ")");
									fd.Data = System.IO.File.ReadAllBytes(args[3]);
									fd.Length = fd.Data.Length;
									Console.WriteLine(" New Size " + fd.Length.ToString());
									break;
								}
							}
							if( !foundfile )
							{
								Console.WriteLine("Did not find file " + args[2]);
							}
						}
						datafile.SaveResourceFile(args[0]);
						break;

					case "add":
						datafile = new ResourceDat(args[0]);
						FileDescriptor fdx = new FileDescriptor();
						fdx.Filename = args[2];
						fdx.Data = System.IO.File.ReadAllBytes(args[3]);
						datafile.Files.Add(fdx);
						datafile.SaveResourceFile(args[0]);
						break;
				}
			}


			Console.WriteLine ();
		}

		public static void WriteUsage()
		{
			Console.WriteLine("Usage:");
			Console.WriteLine("  ftldata <datafile> list");
			Console.WriteLine("  ftldata <datafile> extract <filename>");
			Console.WriteLine("  ftldata <datafile> extractall");
			Console.WriteLine("  ftldata <datafile> replace <internal filename> <contents filename>");
			Console.WriteLine("  ftldata <datafile> add <internal filename> <contents filename>");
		}
	}
}
