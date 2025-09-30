using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Backend.Util.Json.Data.Addressables
{
    [System.Serializable]
    public class AddressAssetData
    {
        public string ToConstantFieldName(int index)
        {
            return ToConstantFieldName(Addresses[index]);
        }

        private string ToConstantFieldName(string address)
        {
            var extension = Path.GetExtension(address);
            if (string.IsNullOrEmpty(extension) == false)
            {
                extension = char.ToUpper(extension[1]) + extension.Substring(2);
            }

            var name = Path.ChangeExtension(address, null);
            name = Regex.Replace(name, @"[^\w]", "_");
            name = Regex.Replace(name, @"_+", "_");
            name = name.Trim('_');

            if (char.IsDigit(name[0]))
            {
                name = "_" + name;
            }

            return name + "_" + extension;
        }

        public List<string> Addresses { get; set; } = new();
    }
}
