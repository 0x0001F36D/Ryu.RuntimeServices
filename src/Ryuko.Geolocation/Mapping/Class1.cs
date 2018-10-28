
namespace Ryuko.Geolocation.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    public static class MappingExtension
    {
        public static TMapping Mapping<TMapping>(this IGeolocation geolocation)
            where TMapping : ISelfMapping<TMapping>, new()
        {
            return new TMapping().MappingTo(geolocation); 
        }
    }


    public class Taiwan : SelfMapping<Taiwan>
    {
        
        private static readonly Dictionary<string, string> s_mapping = new Dictionary<string, string>
        {
            ["Keelung"] = "基隆",
            ["New Taipei"] = "新北",
            ["Taipei"] = "台北",
            ["Yilan"] = "宜蘭",
            ["Taoyuan"] = "桃園",
            ["Hsinchu"] = "新竹",
            ["Miaoli"] = "苗栗",
            ["Taichung"] = "台中",
            ["Changhua"] = "彰化",
            ["Nantou"] = "南投",
            ["Yunlin"] = "雲林",
            ["Chiayi"] = "嘉義",
            ["Tainan"] = "台南",
            ["Kaohsiung"] = "高雄",
            ["Pingtung"] = "屏東",
            ["Taitung"] = "台東",
            ["Hualien"] = "花蓮",
            ["Penghu"] = "澎湖",
            ["Kinmen"] = "金門",
            ["Lienchiang"] = "連江",
            // ["       "] = "        ",
        };
        

        public string City { get; private set; }

        protected override Taiwan MappingTo(IGeolocation geolocation)
        {
            this.City = s_mapping[geolocation.RegionName];
            return this;
        }
    }

    public interface ISelfMapping<T>
    {
        T MappingTo(IGeolocation geolocation);
    }

    public abstract class SelfMapping<T> : ISelfMapping<T>
    {
        public SelfMapping()
        {
        }

        protected abstract T MappingTo(IGeolocation geolocation);
        T ISelfMapping<T>.MappingTo(IGeolocation geolocation) => this.MappingTo(geolocation);
    }
}
