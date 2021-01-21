﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示键值对
    /// </summary>
    [DebuggerDisplay("{Key}={Value}")]
    public struct KeyValue
    {
        /// <summary>
        /// 获取键
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// 获取值
        /// </summary>
        public string? Value { get; }

        /// <summary>
        /// 键值对
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <exception cref="ArgumentNullException"></exception>
        public KeyValue(string key, string? value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            this.Key = key;
            this.Value = value;
        }

        /// <summary>
        /// 从 KeyValuePair 显式转换
        /// </summary>
        /// <param name="keyValue"></param>
        public static explicit operator KeyValue(KeyValuePair<string, string> keyValue)
        {
            return new KeyValue(keyValue.Key, keyValue.Value);
        }
    }
}
