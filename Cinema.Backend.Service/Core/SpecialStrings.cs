namespace Template.Service.Core
{
    /// <summary>
    /// Defines system and/or special strings.
    /// </summary>
    public static class SpecialStrings
    {
        /// <summary>
        /// User: workday - Built-in user for accessing Workday information
        /// </summary>
        public static readonly string TemplateUser = "template";
        
        /// <summary>
        /// Role: sys_admin - Built-in role for administrating all features and data.
        /// </summary>
        public static readonly string SysAdminRole = "sys_admin";
        
        /// <summary>
        /// Role: view_workday - Built-in role for viewing Workday information
        /// </summary>
        public static readonly string ViewTemplateRole = "view_template";
        
        /// <summary>
        /// Role: manage_workday - Built-in role for managing Workday information
        /// </summary>
        public static readonly string ManageTemplateRole = "manage_template";
    }
}
