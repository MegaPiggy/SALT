using UnityEngine;

namespace SALT
{
    /// <summary>
    /// Extensions for <see cref="Character"/>
    /// </summary>
    public static class CharacterExtensions
    {
        /// <summary>
        /// Converts a <see cref="Character"/> to it's correct name.
        /// </summary>
        /// <param name="character">The character you are converting.</param>
        /// <returns>The name of the character</returns>
        public static string ToFriendlyName(this Character character) => Registries.CharacterRegistry.GetNameFromInt((int)character);

        /// <summary>
        /// Converts a <see cref="Character"/> to an <see cref="int"/> value.
        /// </summary>
        /// <param name="character">The character you are converting.</param>
        /// <returns>An <see cref="int"/> value that is equal to the enum</returns>
        public static int ToInt(this Character character) => (int)character;//Registries.CharacterRegistry.GetIntFromName(character.ToFriendlyName());
        
        /// <summary>
        /// Gets a <see cref="Character"/> from a name. 
        /// </summary>
        /// <param name="name">Name of the character.</param>
        /// <returns>A <see cref="Character"/> that has the provided name.</returns>
        public static Character ToCharacter(this string name) => Registries.CharacterRegistry.GetCharacterFromName(name);

        /// <summary>
        /// Gets the <see cref="Character"/> equal to an <see cref="int"/> value. 
        /// </summary>
        /// <param name="id">The number equal to the <see cref="Character"/></param>
        /// <returns>The <see cref="Character"/> equal to the provided <see cref="int"/></returns>
        public static Character ToCharacter(this int id) => Registries.CharacterRegistry.GetFromInt(id);

        /// <summary>
        /// Gets the <see cref="Character"/>'s prefab
        /// </summary>
        /// <param name="character">Id of the prefab</param>
        /// <returns>The character prefab</returns>
        public static GameObject GetPrefab(this Character character) => Registries.CharacterRegistry.GetCharacter(character);

        /// <summary>
        /// Gets the <see cref="Character"/>'s icon
        /// </summary>
        /// <param name="character">Id of the icon</param>
        /// <returns>The character icon</returns>
        public static Sprite GetIcon(this Character character) => Registries.CharacterRegistry.GetIcon(character);

        /// <summary>
        /// Gets the current character pack from the player.
        /// </summary>
        /// <param name="player">The player to check</param>
        /// <returns>The current character pack attached to the player.</returns>
        public static GameObject GetCurrentCharacterPack(this PlayerScript player) => player.currentCharacterPack;

        /// <summary>
        /// Gets the selected <see cref="Character"/> id from a player. 
        /// The selected character is different from the current character.
        /// </summary>
        /// <param name="player">The player to check</param>
        /// <returns>The player's selected <see cref="Character"/></returns>
        public static Character GetSelectedCharacter(this PlayerScript player) => EnumUtils.FromInt<Character>(player.currentCharacter);

        /// <summary>
        /// Gets the <see cref="Character"/> id from a player's <see cref="CharacterPack"/>. 
        /// The current character is different from the selected character.
        /// </summary>
        /// <param name="player">The player to check</param>
        /// <returns>The player's current <see cref="Character"/></returns>
        public static Character GetCurrentCharacter(this PlayerScript player) => player.GetCurrentCharacterPack().GetComponent<CharacterIdentifiable>().Id;// CharacterIdentifiable.GetId(player.GetCurrentCharacterPack());
    }
}
