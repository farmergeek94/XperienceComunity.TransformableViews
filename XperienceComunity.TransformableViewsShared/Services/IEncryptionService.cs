using HBS.TransformableViews;
using HBS.TransformableViews_Experience;

namespace HBS.Xperience.TransformableViewsShared.Services
{
    public interface IEncryptionService
    {
        string DecryptString(string cipherText);
        TransformableViewInfo DecryptView(TransformableViewInfo view);
        string EncryptString(string plainText);
        TransformableViewInfo EncryptView(TransformableViewInfo view);
    }
}