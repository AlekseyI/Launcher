using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CommonClasses.Serializers;

namespace CommonClasses.Request
{
    public interface IRequest
    {

        V Request<T, V>(string url, string method, T data, IAuthSerializer auth) where T : class where V : class;
        T Request<T>(string url, string method, T data, IAuthSerializer auth) where T : class;
        Task<V> RequestAsync<T, V>(string url, string method, T data, IAuthSerializer auth, DownloadProgressChangedEventHandler progressGet, UploadProgressChangedEventHandler progressPost) where T : class where V : class;
        Task<T> RequestAsync<T>(string url, string method, T data, IAuthSerializer auth, DownloadProgressChangedEventHandler progressGet, UploadProgressChangedEventHandler progressPost) where T : class;
    }
}
