using InforceTask.Models;

namespace InforceTask.Services;

public interface IAboutRedactorService
{
	AboutPageContents ChangeAboutPageContentsTo(string contents);
	AboutPageContents GetCurrentAboutPageContents();
}