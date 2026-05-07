using LongDucProjectTest.ServiceReference1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace LongDucProjectTest.Service
{
    public class RealtimeService
    {
        #region FILEDS

        private static volatile RealtimeService instance;

        private static readonly object keyLock = new object();

        public ATSCADAServiceClient client;

        #endregion

        #region CONSTRUCTORS

        public static RealtimeService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (keyLock)
                    {
                        if (instance == null)
                        {
                            instance = new RealtimeService();
                        }
                    }
                }
                return instance;
            }
        }

        private RealtimeService()
        {
            this.client = new ATSCADAServiceClient();
        }

        #endregion

        #region METHODS

        public ResultPackage[] Read(string[] names)
        {
            try
            {
                if (names is null) return default;
                return client.Read(names);
            }
            catch
            {
                return HandleException<ResultPackage[]>();
            }
        }

        public ResultPackage[] Write(WriteParam[] writeParams)
        {
            try
            {
                if (writeParams is null) return default;
                return client.Write(writeParams);
            }
            catch
            {
                return HandleException<ResultPackage[]>();
            }
        }

        private T HandleException<T>()
        {
            if (this.client.State == CommunicationState.Faulted)
            {
                this.client.Abort();
                this.client = new ATSCADAServiceClient();
            }
            return default;
        }

        #endregion

    }
}