
using System;
using System.Security.Cryptography;

namespace hds
{
	
	public class MxoRSA
	{
		
		const string privKeyXML="<RSAKeyValue><Modulus>qMIfEkrXWpRr44ecWMzJHV7Hjg9bnru2PZv3NydzOZ6uab52wET+RoHhIzv+zJb3zBhmETAtsrmNnBXiW7tfqPK0xf6lb9RbvupfnfYSHO5WaEcWEi0JjQRBevg9d8qlETo9Hrfy9PEfpeK1T2WF+xxx73chvBTB12Paa7yT+Ik=</Modulus><Exponent>EQ==</Exponent><P>y1biPGcMKGk2VRKuHJZBvge0A/8YLTM5+gvdiZn2+2sC09iz/zjWa26ARP9ENMA687B6vo9asGtHovQCizWNgw==</P><Q>1HaPHEY726MwhSXUOl9l3Roon/hxOdfEE4ooMSe0z8WYW3+IDXTAbYCpJHMZNIvrTLSEuReuDdNL8W91Kq5wAw==</Q><DP>v2DU7Y4pj3IVBMZJhFEu0Pgw9LPahOTrRbDQgYHZZRlsEq3WldskKOB4uWi4qh5Vmg+ClTugpgqdxotNsDJnEQ==</DP><DQ>V3wcz2g2w9nIr0vP28zttWUfyWZMvXb2YmYQjLX/KGBr6XC/jRH04cuQ8OQZb/1g41lj07502IQuVFsSIKIuHw==</DQ><InverseQ>CnkbOBzROWVuJMlB8YIwswZSkBmhckoz+EjEKqIUSE8cYPnhOr2KNfUnTVFTFLlOzWc4jrrOQPi94rrDae8yPw==</InverseQ><D>RX0b2lsNYYhoqPuauycloq6OZ6v4jKelZKmiB6bVF7nPWLfWi2ez/uovhvqWGAHtkEZIJTH0swEcMTYwB6eBvV5fQPkL1CiZJELi7UGEME9v/UMqsMn/aAs3iqDYn0sR1kC4mZHRRNzIq7l16yufV6XnNaVuwlDMo3OLVWwqWE0=</D></RSAKeyValue>";
		const string privKeyXML2="<RSAKeyValue><Modulus>paFUaHOzouk8tl++bnudYwPn41Jx6wjp+s8RZmf4MCwCvryqBYcmU/ZQBgYPYjTDjNzMaW35zfSwRYhgppQExTTyxEX2HNXQWGd6zPzoAu3Q2JIslrSRgXuHXoAqercvjdjqpQcOyLkXDiWuLzRo18oSYrPUORmpJOsSrPfjKsRyTiB5XPp/Azy1nesfeXfnBi1nCLvaBiQ1xiXGIXSoW4VD9GIFvsRjZcZyU/dqQrMWt8ykyDM08qxt1AzKDCuksS7XJx3SBJ4aggqmh5DMDay+lKYgTAKrpOXIjXKQ+Ib4VNXQVijCzVnQUhW0bmdClNVz+5MTFtMN9mXU2U7opQ==</Modulus><Exponent>EQ==</Exponent><P>uUcMF/eZJf4KAxbGkmOb72rsIRpX1U6YI126SPfZ5vo1LGtlfikSlJktj4iXyRJn7IeSQDHWagth3J+4UzTzhclQqufUzKe1Y0PqLmsoTnh/2e1G1eX4BL0xuQNiZudre47vi7ThbByw3Y9xVnvG1II4CdKrjg058qrXs7BIXUU=</P><Q>5NpiShLNf2X+Q0ASlqjMW/z/JSilMul2d9piQI0DXNPccBdOMGkcvCP0dHuey+0ARgRmkTXWLvBirOUD7O5vyNDYsxAZsFa71Q9/tC3MhDtLUFfkZNLsJ1gGM4KIhWtctlvf3AWQHzVrfhkONLTV/Yac8QyD3d5EVhqglXUqo+E=</Q><DP>Nn5O2d9pR2jVxKxYhWiXRmq9655WEY+WKIT6je6LYg1a7vJpJRsjlR3+V2Ro4Mktn+ujfErktcccyGs2NpcacqSfQVM+lout0ebMaAFmNTJ/9M1REbwbxSiWJ1tZLVMfnM+v3cvJ1IDouaKo3TN2tvkfimsjZgPj3fYDNNl+sgU=</DP><DQ>rwFaOKT3f4o66CHwGNtvGSrhOoh+VBvxLnnSqdU+zoPkzi/wf19wU6MGOvUfFGnxJnvV9pKUunuWwHLk4lv7IRgtPaLmd8na7jkHTYxvN/EbeayfmGUO8Oj1rutZVve/XkZBxl6bYyjZusfdr9WUlLI7x2PsXl6sulCY6sMCfVE=</DQ><InverseQ>f24bB2UyT8Suowp1HbgzA4Bg5BGMWwoPEt8p8EFfLN0haV4XLQY4hXShCBGHf+D7hv+3hNLsgcmnXWUxSv8mZYjNcpMP69JffM73QrCadptczxyfwMjuJ3i8Z98BbOlHPaIiWPbF2tREKhSddaHGW3Jx8S8I83ixel8X+PB2qiM=</InverseQ><D>HTqWbMkfs1ZWAhDlXsqFL5dHCf99g9Rlhp0DEhJY+XEtqTBaPTX3tHbC0+Lzp+sxkVQkEprv2Q0QDEU+O4OIXwlYBITRFCW7WuUVq7Qo8XVC+Qq8kxDOYiTbp0PLQtUIZFN0s7XzjNVea+iIJnKpFwWKxh+8ChOWUc8hadFkUtcWYj2LpSkmZ2ObKqjRIk4MZzjYm13oygPP+vKSOcBdCec7qqZLnnSo8JV84OXLSBxGYhZSenIbheyv0cLrKRRLbXlvvQhaTGJGCDp87d9KHuuSpJ03km7LN3kItgjg0fR9XgEhhOQY2YNQ0qIHRyO5gh4zfXr/Yf/mkCnrehe/cQ==</D></RSAKeyValue>";

		public MxoRSA(){}
		
		public byte[] decryptWithPrivkey(byte[] block){
			RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            rsa.FromXmlString(privKeyXML);
				
			/* True here means OAEP enabled, else fails */
            return rsa.Decrypt(block, true);
		}
		
		public byte[] signWithMD5(byte[] md5fromStructure){
			RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            rsa.FromXmlString(privKeyXML);
			
			return rsa.SignData(md5fromStructure, new MD5CryptoServiceProvider());
			
		}		
		
	}
}
