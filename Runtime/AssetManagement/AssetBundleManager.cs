using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUtilities.AssetManagement
{
    public static class AssetBundleManager
    {
        // A dictionary to hold the AssetBundle references
        static private Dictionary<string, AssetBundleRef> dictAssetBundleRefs;
        static AssetBundleManager()
        {
            dictAssetBundleRefs = new Dictionary<string, AssetBundleRef>();
        }
        // Class with the AssetBundle reference, url and version
        private class AssetBundleRef
        {
            public AssetBundle assetBundle = null;
            public int version;
            public string url;
            public AssetBundleRef(string strUrlIn, int intVersionIn)
            {
                url = strUrlIn;
                version = intVersionIn;
            }
        };

        // Load local AssetBundle
        public static AssetBundle LoadAssetBundle(string path)
        {
            string keyName = path;
            AssetBundleRef abRef;
            if (dictAssetBundleRefs.TryGetValue(keyName, out abRef))
                return abRef.assetBundle;
            else
            {
                var bundle = AssetBundle.LoadFromFile(path);
                if (bundle != null)
                    dictAssetBundleRefs.Add(keyName, new AssetBundleRef(path, 1) { assetBundle = bundle });
                return bundle;
            }
        }
    }
}