namespace DocumentationMarkdownToHtml
{
    using System;
    
    /// <summary>
    /// Helper class that exposes all GUIDs used across VS Package.
    /// </summary>
    internal sealed partial class PackageGuids
    {
        public const string guidAddDocumentationCommandPackageString = "fa6bf923-31c1-43aa-b0dc-24c485190e54";
        public const string guidAddDocumentationCommandPackageCmdSetString = "c0e2f059-bdbc-47a5-999f-b0901f86d7ff";
        public const string guidImagesString = "8062d29e-b47e-4fb6-9c94-94924b03409c";
        public static Guid guidAddDocumentationCommandPackage = new Guid(guidAddDocumentationCommandPackageString);
        public static Guid guidAddDocumentationCommandPackageCmdSet = new Guid(guidAddDocumentationCommandPackageCmdSetString);
        public static Guid guidImages = new Guid(guidImagesString);
    }
    /// <summary>
    /// Helper class that encapsulates all CommandIDs uses across VS Package.
    /// </summary>
    internal sealed partial class PackageIds
    {
        public const int CommandGroup = 0x1020;
        public const int AddDocumentationCommandId = 0x0100;
        public const int OpenInVsCodeCommandId = 0x0200;
        public const int CheckForUpdatesCommandId = 0x0300;
        public const int UpdateGulpFileCommandId = 0x0400;
        public const int UpdateHtmlTemplateCommandId = 0x0500;
        public const int UpdatePackageJsonCommandId = 0x0600;
        public const int UpdateVsCodeTasksJsonCommandId = 0x0700;
        public const int MarkDownIcon = 0x0001;
        public const int VisualStudioIcon = 0x0002;
    }
}
