using System.Collections.Generic;
using MementoNagBot.Models.Misc;
using Microsoft.Extensions.Logging;

namespace MementoNagBot.Services.Translation;

// I'd probably do this differently if it wasn't a very limited set of resources that needed translating
// The interface has been provided to allow for the likely situation that this set expands in the future
public class StaticTranslatedResourceService: ITranslatedResourceService
{
	private readonly ILogger<StaticTranslatedResourceService> _logger;

	private readonly Dictionary<TranslatedResourceCompoundKey, string?> _resourceDictionary = new()
	{
		{new(TranslatedResource.GeneralReminderTemplate, Language.English), "Hi everyone, it's that time of week again, please make sure your Memento is up to date!"},
		{new(TranslatedResource.GeneralReminderTemplate, Language.French), "Bonjour à tous, c'est encore ce moment de la semaine. Merci de vérifier si votre Memento est à jour!"},
		{new(TranslatedResource.GeneralReminderTemplate, Language.Portuguese), "Olá a todos. Já chegou novamente *aquele* momento na semana. Por favor assegurem-se de actualizar o Memento!"},
		{new(TranslatedResource.GeneralReminderTemplate, Language.Spanish), "Hola a todos. Ya ha llegado ese momento de la semana. ¿Puedes asegurarte de que Memento está actualizado?"},
		
		{new(TranslatedResource.GeneralReminderMonthEndTemplate, Language.English), "Hi everyone, tomorrow is month end, could you please make sure that your Memento is up to date and remember to fill it out for tomorrow too!"},
		{new(TranslatedResource.GeneralReminderMonthEndTemplate, Language.French), "Bonjour à tous, demain est le dernier jour du mois, merci de vérifier si votre Memento est à jour et n'oubliez pas de le remplir pour demain aussi!"},
		{new(TranslatedResource.GeneralReminderMonthEndTemplate, Language.Portuguese), "Olá a todos. Já chegou novamente *aquele* momento na semana. Por favor assegurem-se de actualizar o Memento!"},
		{new(TranslatedResource.GeneralReminderMonthEndTemplate, Language.Spanish), "Hola a todos. Mañana es el último día del mes. Asegúrate de que Memento está actualizado incluyedo mañana."},
		
		{new(TranslatedResource.IndividualReminderTemplate, Language.English), "Hi {Person}, it looks like you've forgotten to fill out your Memento. We know you're busy but we really need it to be kept up to date for billing purposes. So please could you ensure it's updated as soon as possible! If you're having difficulties doing so, please reach out to your manager!"},
		{new(TranslatedResource.IndividualReminderTemplate, Language.French), "Salut {Person}, il semble que tu as oublié de remplir ton Memento. On sait que tu es occupé mais on a vraiment besoin que tu le remplisses pour des raisons de facturation. Merci de vérifier si celui-ci est à jour le plus rapidement possible! Si tu as un problème, contactes ton supérieur."},
		{new(TranslatedResource.IndividualReminderTemplate, Language.Portuguese), "Olá {Person}, parece que te esqueceste de actualizar o teu Memento. Sabemos que estás ocupada/o mas, por favor, necessitamos de mantê-lo actualizado por motivos de facturação. Assim sendo, poderias, por favor, garantir que o actualizas assim que possível! Se tiveres alguma dificuldade para fazê-lo por favor contacta o teu manager"},
		{new(TranslatedResource.IndividualReminderTemplate, Language.Spanish), "Hola {Person}, parece que se te ha olvidado rellenar tu Memento. Sabemos lo ocupado que estás pero de verdad que necesitamos que lo mantegas actualizado para poder facturar. Así que, por favor, asegurate de que está actualizado cuanto antes. Si tienes dificultades, no dudes en comentárselo a tu jefe."},
		
		{new(TranslatedResource.IndividualReminderMonthEndTemplate, Language.English), "Hi {Person}, it looks like you've forgotten to fill out your Memento. We know you're busy but we really need it to be kept up to date for billing purposes. So please could you ensure it's updated as soon as possible! Please also remember to fill out tomorrow as it's month end. If you're having difficulties doing so, please reach out to your manager!"},
		{new(TranslatedResource.IndividualReminderMonthEndTemplate, Language.French), "Salut {Person}, il semble que tu as oublié de remplir ton Memento. On sait que tu es occupé mais on a vraiment besoin que tu le remplisses pour des raisons de facturation. Merci de vérifier si celui-ci est à jour le plus rapidement possible, et également de le remplir pour demain car c'est le dernier jour du mois. Si tu as un problème, contactes ton supérieur."},
		{new(TranslatedResource.IndividualReminderMonthEndTemplate, Language.Portuguese), "Olá {Person}, parece que te esqueceste de actualizar o teu Memento. Sabemos que estás ocupada/o mas, por favor, necessitamos de mantê-lo actualizado por motivos de facturação. Assim sendo, poderias, por favor, garantir que o actualizas assim que possível! Lembra-te igualmente de fazê-lo amanhã já que é o fim do mês. Se tiveres alguma dificuldade para fazê-lo por favor contacta o teu manager"},
		{new(TranslatedResource.IndividualReminderMonthEndTemplate, Language.Spanish), "Hola {Person}, parece que se te ha olvidado rellenar Memento. Sabemos que estás ocupado pero de verdad que necesitamos que lo mantengas actualizado para poder facturar. ¡Así que asegurate de que está actualizado cuanto antes! También, asegurate de incluir mañana ya que es final de mes. Si tienes dificultades, coméntaselo a tu jefe. "},
	};

	public StaticTranslatedResourceService(ILogger<StaticTranslatedResourceService> logger)
	{
		_logger = logger;
	}

	public string GetTranslatedString(TranslatedResourceCompoundKey compoundKey)
	{
		if (!_resourceDictionary.TryGetValue(compoundKey, out string? message))
		{
			if (compoundKey.Language is not Language.English)
			{
				_logger.LogWarning("Translation not found for {TranslationKey}. Attempting to fall-back to English...", compoundKey);
				return GetTranslatedString(compoundKey with { Language = Language.English });
			} 
			_logger.LogError("Translation not found {TranslationKey}! Attempting to continue...", compoundKey);
		}

		return message ?? String.Empty;
	}
}
