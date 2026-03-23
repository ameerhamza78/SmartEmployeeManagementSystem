using System.Drawing;

namespace SEMS
{
    public static class ThemeManager
    {
        public static bool IsDarkMode = true;

        // ===== COLORS =====
        public static Color SidebarBack
        {
            get { return IsDarkMode ? Color.FromArgb(30, 30, 30) : Color.FromArgb(245, 245, 245); }
        }

        public static Color SidebarText
        {
            get { return IsDarkMode ? Color.FromArgb(200, 200, 200) : Color.FromArgb(51, 51, 51); }
        }

        public static Color SidebarHover
        {
            get { return IsDarkMode ? Color.FromArgb(44, 44, 44) : Color.FromArgb(224, 224, 224); }
        }

        public static Color SidebarActive
        {
            get { return Color.FromArgb(63, 81, 181); } // same in both modes
        }

        public static Color SidebarActiveText
        {
            get { return Color.White; }
        }
    }
}