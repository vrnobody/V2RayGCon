﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ZipExtractor.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ZipExtractor.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Extracting {0}.
        /// </summary>
        internal static string CurrentFileExtracting {
            get {
                return ResourceManager.GetString("CurrentFileExtracting", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to {0} is still open and it is using "{1}". Please close the process manually and press Retry..
        /// </summary>
        internal static string FileStillInUseMessage
        {
            get
            {
                return ResourceManager.GetString("FileStillInUseMessage", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Unable to update the file!.
        /// </summary>
        internal static string FileStillInUseCaption
        {
            get
            {
                return ResourceManager.GetString("FileStillInUseCaption", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Removing {0}.
        /// </summary>
        internal static string Removing {
            get {
                return ResourceManager.GetString("Removing", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap ZipExtractor {
            get {
                object obj = ResourceManager.GetObject("ZipExtractor", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
    }
}
