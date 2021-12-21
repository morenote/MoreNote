﻿using System.Collections.Generic;

using Fido2NetLib;

namespace MoreNote.Logic.Security.Service
{
    public static class FIDO2Conformance
    {
        private static readonly object _syncRoot = new();

        private static IMetadataService _instance;

        public static IMetadataService MetadataServiceInstance(string cacheDir, string origin)
        {
            if (_instance == null)
            {
                lock (_syncRoot)
                {
                    if (_instance == null)
                    {
                        var repos = new List<IMetadataRepository>
                        {
                            new ConformanceMetadataRepository(null, origin),
                            new FileSystemMetadataRepository(cacheDir)
                        };
                        _instance = new SimpleMetadataService(repos);
                        _instance.Initialize().Wait();
                    }
                }
            }
            return _instance;
        }
    }
}