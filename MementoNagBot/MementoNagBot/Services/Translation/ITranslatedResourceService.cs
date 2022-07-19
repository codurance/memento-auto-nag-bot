using MementoNagBot.Models.Misc;

namespace MementoNagBot.Services.Translation;

public interface ITranslatedResourceService
{
	public string GetTranslatedString(TranslatedResourceCompoundKey compoundKey);
}