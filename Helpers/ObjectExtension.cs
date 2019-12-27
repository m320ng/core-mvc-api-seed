using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;

namespace SeedApi.Helpers {
    public static class ObjectExtension {
        /*
         * Property 값을 가져옴
         * @param obj 객체
         * @param propName Property명
         **/
        public static object GetPropValue(this object obj, string propName) {
            Type type = obj.GetType();
            var prop = type.GetProperty(propName);
            if (prop == null) return null;
            return prop.GetValue(obj, null);
        }

        /*
         * Property 값을 설정
         * @param obj 객체
         * @param propName Property명
         * @param value 값
         **/
        public static void SetPropValue(this object obj, string propName, object value) {
            Type type = obj.GetType();
            var prop = type.GetProperty(propName);
            if (prop != null) prop.SetValue(obj, value, null);
        }

        public static bool IsNullProp(this PropertyInfo obj, object value) {
            bool isnull = false;
            try {
                object o = obj.GetValue(value, null);
                if (o == null) {
                    isnull = true;
                }
            } catch (Exception) {
                isnull = true;
            }
            return isnull;
        }

        /*
         * 객체내부를 덤프
         * @param obj 객체
         **/
        public static string Dump(this object obj) {
            if (obj == null) return "{}";
            return System.Text.Json.JsonSerializer.Serialize(obj);
        }

    }
}
