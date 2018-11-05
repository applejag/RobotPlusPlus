using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Structures;

namespace RobotPlusPlus.Core.Utility
{
	public static class StringUtilities
	{
		private static readonly Bictionary<char, string> escapedCharacters = new Bictionary<char, string>
		{
			{ '\a', @"\a" },
			{ '\b', @"\b" },
			{ '\f', @"\f" },
			{ '\n', @"\n" },
			{ '\r', @"\r" },
			{ '\t', @"\t" },
			{ '\v', @"\v" },
			{ '"', @"\""" },
			{ '\\', @"\\" },
		};

		private static readonly Dictionary<char, string> characterSimplifications = new Dictionary<char, string>
		{
			{ '0', "סه٥ە۵߀०০੦૦ଠ୦௦౦೦ഠ൦๐໐༠ဝ၀ჿᄆᆷዐ០᠐᥆᧐᮰᱐⁰₀ℴ⑩⓪⓿➉ⵔ〇ㅁꓳ꣐ꬽ０" },
			{ '1', "ǀוןا١۱ߊᛁⅠⅼⲒⵏꓲﺍﺎ１ˡᣳבℶ①➀" },
			{ '2', "ᒿꛯ２ᄅᆯㄹ②➁" },
			{ '3', "ƷȜЗӠⳌꝪꞫ３ⴺꓱȝʒӡჳⳍꝫɜзᴈЭ३૩③➂" },
			{ '4', "Ꮞ４ҷӌҶӋ④➃" },
			{ '5', "Ƽ５⑤➄" },
			{ '6', "бᏮⳒ６Ϭ⑥➅" },
			{ '7', "７לﬥ⑦➆" },
			{ '8', "Ȣȣ৪੪８⑧➇" },
			{ '9', "৭੧୨൭ⳊꝮ９⁹ꝰ⑨➈" },
			{ 'A', "ÀȀÁÂẤẦẨĀÃÄǞÅǺĂẶȂĄǍȦǠḀẠẢẰẲẴẮẪẬꜲÆǼǢꜴꜶꜸꜺꜼᎪАӔΑΆᾺἈἍἎἏⰀⱯⱭＡӒ" },
			{ 'B', "ḂḄḆɃƁƂБВℬƄΒⰁЬᏏＢᏴꞴᏰꞚⲂ" },
			{ 'C', "ÇȻĈĆḈĊČＣƇϹСᏟℂℭⲤƆϽↃႠϚꞆ" },
			{ 'D', "ᎠⅅＤԀᏧΔԾՁĎĐƋḊḌḎḐḒƊÐǄƉǱԴⲆДႣⰄ" },
			{ 'E', "ÈÉÊËĒĔĖĘĚƎƏƐƩȄȆȨΈΕΞΣЀЁЄЕѢҼӘԐԵᎬᏋḔḖḘḚḜẸẺẼẾỀỂỄỆἘἙἚἛἝῈℇℰⰅⲈꞒＥ" },
			{ 'F', "ƑͰϜՖᎨᎰḞℱⰗⰪꞘＦ" },
			{ 'G', "ĜĞĠĢƓǤǦǴЃҐӶԌՑᏀᏳⰃＧ" },
			{ 'H', "ĤĦǶȞΉΗЋНҺԊՀᎻᏂḢḤḦḨḪἨἩἪἫἬἭἮἯῊℋℌℍⰘⱧⱵⲎꜦＨ" },
			{ 'I', "ÌÍÎÏĨĪĬĮİĲƖƗǏΊΙΪІЇЫѶҊӀӢӤӸᎥḮỈỊℐℑⰉⰊⰋꙆＩ" },
			{ 'J', "ĴͿЈՅᎫꞲＪ" },
			{ 'K', "ĶǨΚЌКҚҜӃԿᏦḰḲḴKⰍⱩⲔＫ" },
			{ 'L', "ĹĽŁǇȽΛԼᏞḶḸỺℒⰎⱢⳐꝈＬ" },
			{ 'M', "ЉМՊᎷḾṀṂℳⰏⱮⲘＭϺ" },
			{ 'N', "ÑŃŅŇŊƝǸȠͶΝΠϏЊЍИЙЛПՆՈՌṄṆṈṊℕℿⰐⲚⲠＮ" },
			{ 'O', "ÒÓÔÕÖØŌŎŐŒƠǑȮȰΌΏΟΦΩОФӦՓՕṎṒỌỎỐỒỔỖỘỚỜỞỠỢὈὉὌὍὨὩὬὭὮὯΩⰑⲞⲪⳜꙨꝊꝌꝎꝹＯ" },
			{ 'P', "ÞƤǷΡϷРՔᏢṔṖℙⰒⲢＰ" },
			{ 'Q', "ǪɊϘϞԚԳℚꝘＱ" },
			{ 'R', "ŔŘƦȐȒΓГЯҒԻՐᎡᎱᏒṘṚṜṞℛℜℝℾⰓⱤⲄꝚꞞＲ" },
			{ 'S', "ŚŜŞŠȘЅҪՏᏕᏚṠṢṤṦṨＳ" },
			{ 'T', "ŢŤŦƮȚΤТԷᎢᎿṪṬṮⰕⲦꞱＴ" },
			{ 'U', "ÙÚÛÜŨŪŬŮŰŲƯƱƲǓǕǗǙǛȔɄΜԱՄՍṲṴṶṸṺỤỦỨỪỬỮỰⰖＵ" },
			{ 'V', "ɅѴᏙṼṾꝞＶ" },
			{ 'W', "ŴƜΨШЩѠѰԜՒᎳᏇᏔᏯẀẂẄẆẈΏⲰⲼꙌꞶＷ" },
			{ 'X', "ΧХҲӼӾẊẌⲬꞳＸ" },
			{ 'Y', "ÝŶŸƔƳȲɎΎΥΫϒЎУҮᎩᎽẎỲỴỶỸỾὙὛὝὟῨῩῪⲨⲮＹ" },
			{ 'Z', "ŹŻŽƧƵȤΖϨԶᏃẐẒẔℤℨⰈⲌꙄＺ" },
			{ 'a', "àȁáâấầẩāãäǟåǻăặȃąǎȧǡḁạảẚằẳẵắẫậꜳæᴁǽǣꜵꜷꜹꜻꜽᴂаӕαάὰἀἅᾶἆἇⰰªɐɑɒａӓ" },
			{ 'b', "ḃḅḇƀᵬᶀɓƃвʙƅβⰱьｂꮟᏼϐꞵɞꞛⲃ" },
			{ 'c', "çȼĉćḉċčɕｃƈϲсⲥɔͻↄⴀᴄꮯᴐςϛꞇʗ" },
			{ 'd', "ᴅꭰｄԁⅆδծẟձꝺďđƌȡḋḍḏḑḓɗðǆɖȸǳդⲇдⴃⰴ" },
			{ 'e', "èéêëēĕėęěǝȅȇȩɘəɚɛɝʚέεξσϵеэѐёєѣҽәԑեᴇḕḗḙḛḝẹẻẽếềểễệἐἑἒἓἕὲℯⅇⰵⲉꞓꬲꭼꮛｅ" },
			{ 'f', "ſƒʩͱϝֆᶂḟẝⱇⱚꞙꬵﬀﬁﬂﬃﬄｆ" },
			{ 'g', "ĝğġģƍǥǧǵɠɡɢɤʛѓґӷԍցᏻᶃℊⰳꮐｇ" },
			{ 'h', "ĥħƕȟɥɦɧʜʮʯήηнћһԋիհḣḥḧḩḫẖἠἡἢἣἤἥἦἧὴᾐᾑᾒᾓᾔᾕᾖᾗῂῃῄῆῇℎℏⱈⱨⱶⲏꜧꞕꮋｈ" },
			{ 'i', "ìíîïĩīĭįıĳǐɨɩɪΐίιϊыіїѷҋӏӣӥӹḯỉịιℹⅈⰹⰺⰻꙇꭵｉ" },
			{ 'j', "ĵȷɟʝϳјյᴊⅉꭻｊ" },
			{ 'k', "ķĸǩκϰкќқҝӄկᴋḱḳḵⰽⱪⲕⳤꮶｋ" },
			{ 'l', "ĺľłƚǉɫɬɮʟλլḷḹỻℓⰾⳑꝉꮮｌ" },
			{ 'm', "ɱʍϻмљᴍᴟᵯḿṁṃⰿⲙꭑꭠꮇｍ" },
			{ 'n', "ñńņňŋƞǹȵɲɳɴͷϖϗийлпњѝնոռᴎᴨᴫṅṇṉṋℼⱀⲛⲡｎ" },
			{ 'o', "ºòóôõöøōŏőœơǒǫȯȱɶɸʘοφόϕϙофӧփօᴏᴑᴕṏṓọỏốồổỗộớờởỡợὀὁὄὅⱁⲟⲫⳝꙩꝋꝍꝏｏ" },
			{ 'p', "þƥƿπρϱϸрքᴘᴩṕṗⱂⲣꮲｐ" },
			{ 'q', "ɋʠϟԛգզᶐꝙｑ" },
			{ 'r', "ŕřȑȓɹɺɻɼɽɾʀгяғրᴙᴦṙṛṝṟⱃⲅꝛꭇꭈꭱꮁꮢｒ" },
			{ 's', "ßśŝşšƽșʂʃѕҫտṡṣṥṧṩꜱꭍꮪﬆｓ" },
			{ 't', "ţťŧƫțʈτтէᴛṫṭṯẗⱅⲧꭲﬅｔ" },
			{ 'u', "µùúûüũūŭůűųưǔǖǘǚǜȕʉʊʋΰμυϋύմսᴜᴝᴞᵫṳṵṷṹṻụủứừửữựὐὑὒὓὔὕὖὗὺῠῡῢῦῧⱆꞟꭎꭒｕ" },
			{ 'v', "ʌνѵᴠᶌṽṿꝟꮩｖ" },
			{ 'w', "ŵɯɰʬψωшщѡѱԝապւᴡẁẃẅẇẉẘώῴῶⲱⲽꙍꞷꮃｗώὠὡὤὥὦὧᾠᾡᾤᾥᾦᾧῳῷ" },
			{ 'x', "χхҳӽӿᶍẋẍⲭꭓꭕｘ" },
			{ 'y', "ýÿŷƴȳɏɣʎʏγуўүẏỳỵỷỹỿℽⲩⲯꭚｙ" },
			{ 'z', "źżžƨƶȥʐʑζϩᴢᴤẑẓẕⰸⲍꙅꮓｚ" },
		};

		[NotNull, Pure]
		public static string EscapeString([NotNull] this string text)
		{
			var escaped = new StringBuilder();

			foreach (char c in text)
			{
				if (escapedCharacters.TryGetValue(c, out string e))
					escaped.Append(e);
				else
					escaped.Append(c);
			}

			return escaped.ToString();
		}

		[Pure]
		public static bool EndsWith([NotNull] this string value, char c, bool ignoreCase)
		{
			if (value.Length == 0)
				return false;

			if (ignoreCase)
				return char.ToUpperInvariant(value[value.Length - 1]) == char.ToUpperInvariant(c);

			return value[value.Length - 1] == c;
		}

		[NotNull, Pure]
		public static string EscapeIdentifier([NotNull] this string id)
		{
			id = new string(Array.ConvertAll(id.ToCharArray(), TryEscapeIdentifierCharacter));
			if (id.Length > 0 && char.IsDigit(id[0])) id = $"_{id}";
			return string.Join("_", Regex.Split(id, @"[^_a-zA-Z0-9]+"));
		}

		[Pure]
		public static char TryEscapeIdentifierCharacter(char c)
		{
			if (c == '_'
				|| (c >= '0' && c <= '9')
			    || (c >= 'A' && c <= 'Z')
			    || (c >= 'a' && c <= 'z'))
				return c;

			foreach (KeyValuePair<char, string> pair in characterSimplifications)
			{
				if (pair.Value.IndexOf(c) != -1)
					return pair.Key;
			}

			return c;
		}

        [Pure]
	    public static int IndexOfFirstMismatch(this string s, string other)
	    {
	        string first = s.Length < other.Length ? s : other;
	        string second = s.Length > other.Length ? s : other;
            
	        for (int counter = 0; counter < first.Length; counter++)
	        {
	            if (first[counter] != second[counter])
	            {
	                return counter;
	            }
	        }
	        return -1;
	    }
    }
}