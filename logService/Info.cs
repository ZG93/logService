using logService.Model;
using System.Collections.Generic;

namespace logService
{
    public struct Code
    {
        public static int OK = 0;
        public static int NotFindType = -1002;
        public static int SystemError = -1000;
    }
    public class RetInfo<T>
    {
        public int code { get; set; }
        public string message { get; set; }
        public T data { get; set; }
    }

    public class SearchInfo
    {
        public PageInfo pageInfo { get; set; }

        public List<LogInfo> records { get; set; }
    }

    public class PageInfo
    {
        public int page { get; set; }
        public int count { get; set; }
        public long totalpage { get; set; }
        public long total { get; set; }
    }

}
