using System.Collections.Generic;
using Baracuda.Bedrock.Odin;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace Baracuda.Utilities.Editor.Odin
{
    [UsedImplicitly]
    public class FoldoutPropertyProcessor : OdinPropertyProcessor
    {
        private FoldoutAttribute _activeFoldoutAttribute;

        public override void ProcessMemberProperties(List<InspectorPropertyInfo> propertyInfos)
        {
            var isFirstFoldout = true;
            for (var index = 0; index < propertyInfos.Count; index++)
            {
                var attributes = propertyInfos[index].GetEditableAttributesList();

                var exitAttribute = attributes.GetAttribute<ExitFoldoutAttribute>();
                var foldoutAttribute = attributes.GetAttribute<FoldoutAttribute>();

                if (foldoutAttribute is not null)
                {
                    _activeFoldoutAttribute = foldoutAttribute;
                    if (isFirstFoldout && index > 0)
                    {
                        propertyInfos[index - 1].GetEditableAttributesList().Add(new PropertySpaceAttribute(0, 8));
                    }
                    isFirstFoldout = false;
                }

                if (exitAttribute is not null)
                {
                    attributes.Add(new SpaceAttribute());
                    _activeFoldoutAttribute = null;
                }

                if (_activeFoldoutAttribute is not null)
                {
                    attributes.Add(_activeFoldoutAttribute);
                }
            }
        }
    }
}