namespace CSProjFS.Native
{
    internal static partial class ProjectedFSLib
    {
        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/ne-projectedfslib-prj_callback_data_flags
        [Flags]
        internal enum PRJ_CALLBACK_DATA_FLAGS
        {
            PRJ_CB_DATA_FLAG_ENUM_RESTART_SCAN = 0x00000001,
            PRJ_CB_DATA_FLAG_ENUM_RETURN_SINGLE_ENTRY = 0x00000002
        }

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/ne-projectedfslib-prj_complete_command_type
        internal enum PRJ_COMPLETE_COMMAND_TYPE
        {
            PRJ_COMPLETE_COMMAND_TYPE_NOTIFICATION = 1,
            PRJ_COMPLETE_COMMAND_TYPE_ENUMERATION = 2
        }

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/ne-projectedfslib-prj_file_state
        internal enum PRJ_FILE_STATE
        {
            PRJ_FILE_STATE_PLACEHOLDER = 0x00000001,
            PRJ_FILE_STATE_HYDRATED_PLACEHOLDER = 0x00000002,
            PRJ_FILE_STATE_DIRTY_PLACEHOLDER = 0x00000004,
            PRJ_FILE_STATE_FULL = 0x00000008,
            PRJ_FILE_STATE_TOMBSTONE = 0x00000010
        }

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/ne-projectedfslib-prj_notification
        internal enum PRJ_NOTIFICATION
        {
            PRJ_NOTIFICATION_FILE_OPENED = 0x00000002,
            PRJ_NOTIFICATION_NEW_FILE_CREATED = 0x00000004,
            PRJ_NOTIFICATION_FILE_OVERWRITTEN = 0x00000008,
            PRJ_NOTIFICATION_PRE_DELETE = 0x00000010,
            PRJ_NOTIFICATION_PRE_RENAME = 0x00000020,
            PRJ_NOTIFICATION_PRE_SET_HARDLINK = 0x00000040,
            PRJ_NOTIFICATION_FILE_RENAMED = 0x00000080,
            PRJ_NOTIFICATION_HARDLINK_CREATED = 0x00000100,
            PRJ_NOTIFICATION_FILE_HANDLE_CLOSED_NO_MODIFICATION = 0x00000200,
            PRJ_NOTIFICATION_FILE_HANDLE_CLOSED_FILE_MODIFIED = 0x00000400,
            PRJ_NOTIFICATION_FILE_HANDLE_CLOSED_FILE_DELETED = 0x00000800,
            PRJ_NOTIFICATION_FILE_PRE_CONVERT_TO_FULL = 0x00001000
        }

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/ne-projectedfslib-prj_notify_types
        internal enum PRJ_NOTIFY_TYPES : uint
        {
            PRJ_NOTIFY_NONE = 0x00000000,
            PRJ_NOTIFY_SUPPRESS_NOTIFICATIONS = 0x00000001,
            PRJ_NOTIFY_FILE_OPENED = 0x00000002,
            PRJ_NOTIFY_NEW_FILE_CREATED = 0x00000004,
            PRJ_NOTIFY_FILE_OVERWRITTEN = 0x00000008,
            PRJ_NOTIFY_PRE_DELETE = 0x00000010,
            PRJ_NOTIFY_PRE_RENAME = 0x00000020,
            PRJ_NOTIFY_PRE_SET_HARDLINK = 0x00000040,
            PRJ_NOTIFY_FILE_RENAMED = 0x00000080,
            PRJ_NOTIFY_HARDLINK_CREATED = 0x00000100,
            PRJ_NOTIFY_FILE_HANDLE_CLOSED_NO_MODIFICATION = 0x00000200,
            PRJ_NOTIFY_FILE_HANDLE_CLOSED_FILE_MODIFIED = 0x00000400,
            PRJ_NOTIFY_FILE_HANDLE_CLOSED_FILE_DELETED = 0x00000800,
            PRJ_NOTIFY_FILE_PRE_CONVERT_TO_FULL = 0x00001000,
            PRJ_NOTIFY_USE_EXISTING_MASK = 0xFFFFFFFF
        }

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/ne-projectedfslib-prj_placeholder_id
        internal enum PRJ_PLACEHOLDER_ID
        {
            PRJ_PLACEHOLDER_ID_LENGTH = 128
        }

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/ne-projectedfslib-prj_startvirtualizing_flags
        internal enum PRJ_STARTVIRTUALIZING_FLAGS
        {
            PRJ_FLAG_NONE = 0x00000000,
            PRJ_FLAG_USE_NEGATIVE_PATH_CACHE = 0x00000001
        }

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/ne-projectedfslib-prj_update_failure_causes
        internal enum PRJ_UPDATE_FAILURE_CAUSES
        {
            PRJ_UPDATE_FAILURE_CAUSE_NONE = 0x00000000,
            PRJ_UPDATE_FAILURE_CAUSE_DIRTY_METADATA = 0x00000001,
            PRJ_UPDATE_FAILURE_CAUSE_DIRTY_DATA = 0x00000002,
            PRJ_UPDATE_FAILURE_CAUSE_TOMBSTONE = 0x00000004,
            PRJ_UPDATE_FAILURE_CAUSE_READ_ONLY = 0x00000008
        }

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/ne-projectedfslib-prj_update_types
        internal enum PRJ_UPDATE_TYPES
        {
            PRJ_UPDATE_NONE = 0x00000000,
            PRJ_UPDATE_ALLOW_DIRTY_METADATA = 0x00000001,
            PRJ_UPDATE_ALLOW_DIRTY_DATA = 0x00000002,
            PRJ_UPDATE_ALLOW_TOMBSTONE = 0x00000004,
            PRJ_UPDATE_RESERVED1 = 0x00000008,
            PRJ_UPDATE_RESERVED2 = 0x00000010,
            PRJ_UPDATE_ALLOW_READ_ONLY = 0x00000020,
            PRJ_UPDATE_MAX_VAL
        }

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/ne-projectedfslib-prj_ext_info_type
        internal enum PRJ_EXT_INFO_TYPE : int
        {
            PRJ_EXT_INFO_TYPE_SYMLINK = 1
        }
    }
}
