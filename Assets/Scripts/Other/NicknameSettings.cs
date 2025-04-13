using System;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "NicknameSettings", menuName = "Scripts/NicknameSettings")]
public class NicknameSettings : ScriptableObject
{
    public string defaultNick;
    [Space]
    [MinValue(0)] public int minCharacters;
    [MinValue(0)] public int maxCharacters;
    [Space]
    public bool removeMultipleSpaces;
    public bool replaceSpaces;
    [ShowIf(nameof(replaceSpaces))]
    public string spacesReplace = "_";
    [Space, Tooltip("If true, any nickname error will return a false")]
    public bool agressiveValidation;
    

    public bool TryValidateNickname(string nickname, out string newNickname)
    {
        newNickname = defaultNick;

        try
        {
            if (string.IsNullOrWhiteSpace(nickname))
                return !agressiveValidation;

            var chars = nickname.ToCharArray();

            if (chars.Length < minCharacters)
                return !agressiveValidation;

            if (chars.Length > maxCharacters)
            {
                if (agressiveValidation)
                    return false;
                else
                    Array.Resize(ref chars, maxCharacters);
            }
                


            newNickname = new(chars);

            if (removeMultipleSpaces)
            {
                newNickname = string.Join(" ", newNickname.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            }

            if (replaceSpaces)
            {
                newNickname = newNickname.Replace(" ", spacesReplace);
            }

            return true;
        }
        catch (Exception exp)
        {
            Debug.LogException(exp);
            return false;
        }
    }
}
