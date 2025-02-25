﻿#define DISABLE_CACHE // TODO disabled during development

using System;
using System.Collections.Generic;
using Scriban;
using UnityEngine;

namespace UnityEditor.AI.Planner.CodeGen
{
    class CodeRenderer
    {
        Dictionary<TextAsset, Template> m_TemplateCache = new Dictionary<TextAsset, Template>();

        public string RenderTemplate(TextAsset templateAsset, object templateData)
        {
            var template = GetCachedTemplate(templateAsset);
            return template.Render(templateData);
        }

        private Template GetCachedTemplate(TextAsset templateAsset)
        {
            m_TemplateCache.TryGetValue(templateAsset, out var template);

#if !DISABLE_CACHE
        if (template != null)
	        return template;
#endif

            try
            {
                template = Template.Parse(templateAsset.text);
                if (template != null)
                    m_TemplateCache[templateAsset] = template;
            }
            catch (Exception e)
            {
                Debug.LogError("Exception while trying to parse Scriban template." + e.ToString());
            }
            return template;
        }
    }
}
