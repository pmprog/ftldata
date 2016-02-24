using System;
using System.IO;
using System.Collections.Generic;

namespace ftldata
{
	public class FileDescriptor
	{
		public Int32 Start;
		public String Filename;
		public Int32 Length;
		public Byte[] Data;
	}

	public class ResourceDat
	{

		public List<FileDescriptor> Files = new List<FileDescriptor>();

		public ResourceDat()
		{
		}

		public ResourceDat( String Filename )
		{
			LoadResourceFile( Filename );
		}

		public void LoadResourceFile( String Filename )
		{
			Int32 filecount;
			FileStream filehandle;
			BinaryReader filereader;
			List<Int32> offsetlist = new List<Int32>();
			Int32 offsetcurrent = 0;
			FileDescriptor newdescriptor;
			Int32 stringlength;
			Byte[] stringbytes;
			
			filehandle = new FileStream( Filename, FileMode.Open );
			filereader = new BinaryReader( filehandle );
	
			filecount = filereader.ReadInt32();
			for( UInt32 fileindex = 1; fileindex < filecount; fileindex++ )
			{
				offsetcurrent = filereader.ReadInt32();
				offsetlist.Add( offsetcurrent );
			}
	
			foreach( UInt32 offsetworking in offsetlist )
			{
				if( offsetworking == 0 )
				{
					//newdescriptor = new FileDescriptor();
					//newdescriptor.Filename = "";
					//newdescriptor.Length = -1;
					newdescriptor = null;
				}
				else
				{
					filehandle.Seek( offsetworking, SeekOrigin.Begin );
					newdescriptor = new FileDescriptor();
					newdescriptor.Length = filereader.ReadInt32();
					stringlength = filereader.ReadInt32();
					stringbytes = filereader.ReadBytes( stringlength );
					newdescriptor.Filename = System.Text.ASCIIEncoding.ASCII.GetString( stringbytes );
					newdescriptor.Data = filereader.ReadBytes( newdescriptor.Length );
				}

				if (newdescriptor != null)
				{
					Files.Add(newdescriptor);
				}
			}

			Console.WriteLine("Load: " + Filename + " (Size " + filehandle.Length + ")");
			filereader.Close();
			filehandle.Close();
		}

		public void SaveResourceFile( String Filename )
		{
			FileStream filehandle;
			BinaryWriter filewriter;
			Int32 filepointer;
			
			filehandle = new FileStream( Filename, FileMode.Create );
			filewriter = new BinaryWriter( filehandle );
			
			filewriter.Write( Files.Count );
			
			filepointer = (Files.Count * 4) + 4;
			
			foreach( FileDescriptor descriptor in Files )
			{
				if( descriptor.Length < 0 )
				{
					filepointer = 0;
					filewriter.Write( filepointer );
				}
				else
				{
					filewriter.Write( filepointer );
					filepointer += 8 + descriptor.Filename.Length + descriptor.Data.Length;
				}
			}
			
			foreach( FileDescriptor descriptor in Files )
			{
				if( descriptor.Length >= 0 )
				{
					filewriter.Write( descriptor.Data.Length );
					filewriter.Write( descriptor.Filename.Length );
					filewriter.Write( System.Text.ASCIIEncoding.ASCII.GetBytes( descriptor.Filename ) );
					filewriter.Write( descriptor.Data );
				}
			}

			filewriter.Flush();
			Console.WriteLine("Save: " + Filename + " (Size " + filehandle.Length + ")");
			filewriter.Close();
			filehandle.Close();
		}
	}
}

