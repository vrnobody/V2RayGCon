﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Pacman.Resources.Langs {
    using System;
    
    
    /// <summary>
    ///   一个强类型的资源类，用于查找本地化的字符串等。
    /// </summary>
    // 此类是由 StronglyTypedResourceBuilder
    // 类通过类似于 ResGen 或 Visual Studio 的工具自动生成的。
    // 若要添加或移除成员，请编辑 .ResX 文件，然后重新运行 ResGen
    // (以 /str 作为命令选项)，或重新生成 VS 项目。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class I18N {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal I18N() {
        }
        
        /// <summary>
        ///   返回此类使用的缓存的 ResourceManager 实例。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Pacman.Resources.Langs.I18N", typeof(I18N).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   重写当前线程的 CurrentUICulture 属性，对
        ///   使用此强类型资源类的所有资源查找执行重写。
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
        ///   查找类似 Delete this item? 的本地化字符串。
        /// </summary>
        internal static string DeleteThis {
            get {
                return ResourceManager.GetString("DeleteThis", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Delete {0}? 的本地化字符串。
        /// </summary>
        internal static string DeleteTpl {
            get {
                return ResourceManager.GetString("DeleteTpl", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Pack multiple servers into one server. 的本地化字符串。
        /// </summary>
        internal static string Description {
            get {
                return ResourceManager.GetString("Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Done. 的本地化字符串。
        /// </summary>
        internal static string Done {
            get {
                return ResourceManager.GetString("Done", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Fail! 的本地化字符串。
        /// </summary>
        internal static string Fail {
            get {
                return ResourceManager.GetString("Fail", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Name must not null. 的本地化字符串。
        /// </summary>
        internal static string NameCanNotBeNull {
            get {
                return ResourceManager.GetString("NameCanNotBeNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Parameter is null. 的本地化字符串。
        /// </summary>
        internal static string NullParam {
            get {
                return ResourceManager.GetString("NullParam", resourceCulture);
            }
        }
    }
}
