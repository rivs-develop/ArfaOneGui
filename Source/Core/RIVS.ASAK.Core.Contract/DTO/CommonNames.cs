namespace RIVS.ASAK.Core.Contract.DTO
{
    /// <summary>
    /// Класс общих Xml имен.
    /// </summary>
    public static class CommonNames
    {
        /// <summary>
        /// XML элементы. 
        /// </summary>
        public static class Elements
        {
            /// <summary>
            /// Корневой узел команд.
            /// </summary>
            public const string Commands = "Commands";

            /// <summary>
            /// Команда для создания.
            /// </summary>
            public const string CommandCreate = "Create";

            /// <summary>
            /// Команда для обновления.
            /// </summary>
            public const string CommandUpdate = "Update";

            /// <summary>
            /// Команда для удаления.
            /// </summary>
            public const string CommandDestroy = "Destroy";

            /// <summary>
            /// Идентификатор элемента
            /// </summary>
            public const string Id = "Id";

            /// <summary>
            /// Имя элемента
            /// </summary>
            public const string Name = "Name";

            /// <summary>
            /// Assembly-qualified name of the type 
            /// </summary>
            public const string AssemblyQualifiedName = "AssemblyQualifiedName";
        }
    }
}
