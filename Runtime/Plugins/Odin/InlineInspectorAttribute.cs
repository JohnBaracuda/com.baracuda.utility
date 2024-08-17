using Sirenix.OdinInspector;
using System;

namespace Baracuda.Bedrock.Odin
{
    [Required]
    [InlineEditor(InlineEditorModes.GUIOnly, InlineEditorObjectFieldModes.Foldout)]
    [IncludeMyAttributes]
    public class InlineInspectorAttribute : Attribute
    {
    }

    [EnableGUI]
    [ShowInInspector]
    [IncludeMyAttributes]
    [InlineEditor(InlineEditorModes.GUIOnly, InlineEditorObjectFieldModes.CompletelyHidden)]
    public class InlineFoldout : Attribute
    {
    }
}