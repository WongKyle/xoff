using System;
using DBreeze;
using System.IO;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using DBreeze.Utils;
using Newtonsoft.Json;

namespace XOFF.DBreeze
{

	public class DBReezeConnectionProvider : IDBreezeConnectionProvider, IDisposable
	{
		
		readonly string _path;
        private Semaphore _semaphore;
	    private int i;

	    public DBReezeConnectionProvider()
		{
            CustomSerializator.ByteArraySerializator = (object o) => {
                try
                {
                    var str = JsonConvert.SerializeObject(o);
                    return System.Text.Encoding.UTF8.GetBytes(str);
                }
                catch (Exception ex)
                {
                    throw;
                }
            };
            CustomSerializator.ByteArrayDeSerializator = (byte[] bt, Type t)
                =>
            {
                try
                {
                    var str = System.Text.Encoding.UTF8.GetString(bt);
                    return JsonConvert.DeserializeObject(str, t);
                }
                catch (Exception ex)
                {
                    throw;
                }

            };

            _semaphore = new Semaphore(1,1);

			var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
			 var libraryPath = Path.Combine(documentsPath, "..", "Library");
			_path = Path.Combine(libraryPath, "DBreezeFiles");
		}

		//public DBReezeConnectionProvider(string path = null)
		//{
		//	if (path == null)
		//	{
		//		var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
		//		var libraryPath = Path.Combine(documentsPath, "..", "Library");
		//		_path = Path.Combine(libraryPath, "DBreezeFiles");
		//	}

		//	_path = path;
		//}




		public DBreezeEngine Engine
		{
		    get
		    {
		        i++;
                Debug.WriteLine(string.Format("###### Getting Engine {0} SEMAPHORE: ######", i));
                return new DBreezeEngine(_path);
		    }
		}

        public bool WaitOne([CallerMemberName]string name = "")
        {
            Debug.WriteLine(string.Format("###### WAITING SEMAPHORE: {0} ######", name));
            return _semaphore.WaitOne();
        }
        public void Release([CallerMemberName] string name = "")
        {
            Debug.WriteLine(string.Format("###### RELEASING SEMAPHORE: {0} ######", name));
            _semaphore.Release();
        }


        public void Dispose()
		{
			Debug.WriteLine("Dispose Of Engine");
            _semaphore.Dispose();
		}
	}
}