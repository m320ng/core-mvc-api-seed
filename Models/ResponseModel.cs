


namespace SeedApi.Models {
    public class ResponseModel {
        public bool Success { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

        public static ResponseModel Ok(string message = "요청이 처리되었습니다.", string code = null, object data = null) {
            return new ResponseModel { Success = true, Message = message, Code = code };
        }
        public static ResponseModel Ok(object data) {
            return new ResponseModel { Success = true, Message = "요청이 처리되었습니다.", Code = null };
        }

        public static ResponseModel Error(string message = null, string code = null) {
            return new ResponseModel { Success = false, Message = message, Code = code };
        }
    }

}