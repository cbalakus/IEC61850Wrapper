using System;
using System.Collections.Generic;
using System.Text;

namespace lib61850
{

    public enum IedError
    {
        OK = 0,
        NOT_CONNECTED = 1,
        ALREADY_CONNECTED = 2,
        CONNECTION_LOST = 3,
        SERVICE_NOT_SUPPORTED = 4,
        CONNECTION_REJECTED = 5,
        USER_PROVIDED_INVALID_ARGUMENT = 10,
        ENABLE_REPORT_FAILED_DATASET_MISMATCH = 11,
        OBJECT_REFERENCE_INVALID = 12,
        UNEXPECTED_VALUE_RECEIVED = 13,
        TIMEOUT = 20,
        ACCESS_DENIED = 21,
        OBJECT_DOES_NOT_EXIST = 22,
        OBJECT_EXISTS = 23,
        OBJECT_ACCESS_UNSUPPORTED = 24,
        TYPE_INCONSISTENT = 25,
        TEMPORARILY_UNAVAILABLE = 26,
        OBJECT_UNDEFINED = 27,
        INVALID_ADDRESS = 28,
        HARDWARE_FAULT = 29,
        TYPE_UNSUPPORTED = 30,
        OBJECT_ATTRIBUTE_INCONSISTENT = 31,
        OBJECT_VALUE_INVALID = 32,
        OBJECT_INVALIDATED = 33,
        UNKNOWN = 99
    }
    public enum TriggerOptions
    {
        NONE = 0,
        DATA_CHANGED = 1,
        QUALITY_CHANGED = 2,
        DATA_UPDATE = 4,
        INTEGRITY = 8,
        GI = 16
    }
    public enum ReportOptions
    {
        NONE = 0,
        SEQ_NUM = 1,
        TIME_STAMP = 2,
        REASON_FOR_INCLUSION = 4,
        DATA_SET = 8,
        DATA_REFERENCE = 16,
        BUFFER_OVERFLOW = 32,
        ENTRY_ID = 64,
        CONF_REV = 128,
        SEGMENTATION = 256,
        ALL = SEQ_NUM | TIME_STAMP | REASON_FOR_INCLUSION | DATA_SET | DATA_REFERENCE |
            BUFFER_OVERFLOW | ENTRY_ID | CONF_REV | SEGMENTATION
    }
}
