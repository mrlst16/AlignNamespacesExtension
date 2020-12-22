using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AlignNamespacesExtension.Interfaces.Loaders;
using AlignNamespacesExtension.Models.DTO.NamespaceAlignment;
using AlignNamespacesExtension.Models.DTO.Requests;
using AlignNamespacesExtension.Models.DTO.Responses;
using CommonCore.Interfaces.Utilities;
using CommonCore.Models.Responses;
using CommonCore.Standard;
using CommonCore.Standard.Extensions;

namespace AlignNamespacesExtension.BLL.Loaders
{
    public class NamespaceReplacementLoader : INamespaceReplacementLoader
    {
        private readonly IMapper<Match, string, string, NamespaceReplacement> _namespaceReplacementConverter;


        public NamespaceReplacementLoader(
            IMapper<Match, string, string, NamespaceReplacement> namespaceReplacementConverter
            )
        {
            _namespaceReplacementConverter = namespaceReplacementConverter;
        }

        public async Task<ReplaceNamespacesResponse> ReplaceNamespaces(ReplaceNamespaceRequest request)
        {
            var replacements = new List<NamespaceReplacement>();

            var result = request.Text;
            Regex namespaceRegex = CommonRegex.NamespaceRegex();

            Match match = null;
            int startNext = 0;
            var replacementString = "namespace "
                    + request.NewNamespace
                    + System.Environment.NewLine
                    + "{";

            while ((match = namespaceRegex.Match(result, startNext)).Success)
            {
                var replacement = await _namespaceReplacementConverter.Map(match, request.FileName, request.NewNamespace);
                replacements.Add(replacement);

                result = namespaceRegex.Replace(
                    result,
                    replacementString,
                    match.Length,
                    match.Index
                );

                var endOfNamespace = result.IndexOfClosingCharacter('{', '}', startNext);
                startNext = endOfNamespace;
            }

            return new ReplaceNamespacesResponse()
            {
                Data = result,
                Replacements = replacements,
                Sucess = true
            };
        }

        public async Task<MethodResponse<string>> ReplaceUsings(ReplaceUsingsRequest request)
        {
            var text = request.Text;
            bool anyReplaced = false;
            foreach (var item in request.Replacements)
            {
                var regex = CommonRegex.UsingNamespaceRegex(item.OriginalNamespace);
                var matches = regex.Matches(text);

                if (matches.Count < 1) continue;

                anyReplaced = true;
                text = regex.Replace(text, $"using {item.NewNamespace};");
            }
            return new MethodResponse<string>()
            {
                Data = text,
                Sucess = anyReplaced
            };
        }

    }
}
