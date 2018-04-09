// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration
{
    /// <summary>
    /// Prepares files for generation by copying them locally.
    /// </summary>
    internal class FilePreparer : IDisposable
    {
        private string directoryToCopyTo;

        /// <summary>
        /// Copies the given assembly to the private bin directory.
        /// </summary>
        /// <param name="filePaths">The file paths to copy locally.</param>
        /// <returns>The new file path.</returns>
        public List<string> CopyFileToPrivateBin(IList<string> filePaths)
        {
            List<string> newFilePaths = new List<string>();

            foreach (string filePath in filePaths)
            {
                var domain = AppDomain.CurrentDomain;

                var fullPath = Path.GetFullPath(filePath.Trim());

                // The privateBinPath is a ; seperated list of paths located in the base path of the 
                // application where the CLR will attempt to locate assemblies during the load process.
                // Here we add the location where we will copy dlls to.
                var privateBinPath = string.IsNullOrWhiteSpace(domain.SetupInformation.PrivateBinPath)
                    ? "DefaultGenerationBin"
                    : domain.SetupInformation.PrivateBinPath + ";DefaultGenerationBin";

                directoryToCopyTo = Path.Combine(domain.BaseDirectory, privateBinPath);
                Directory.CreateDirectory(directoryToCopyTo);

                var newFilePath = Path.Combine(directoryToCopyTo, Path.GetFileName(filePath));

                // Manually copy file to location where CLR will be able to locate it.
                File.Copy(fullPath, newFilePath, overwrite: true);
                newFilePaths.Add(newFilePath);
            }

            return newFilePaths;
        }

        /// <summary>
        /// Cleans up the directoy created.
        /// </summary>
        public void Dispose()
        {
            // Empty private bin of assemblies
            var files = new DirectoryInfo(directoryToCopyTo).GetFiles();

            foreach (FileInfo file in files)
            {
                file.Delete();
            }
        }
    }
}