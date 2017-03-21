using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace PairedInterfaces.Analyzers
{
	[ExportCodeFixProvider(LanguageNames.CSharp)]
	[Shared]
	public sealed class EnsurePairedInterfacesExistCodeFix
		: CodeFixProvider
	{
		public override ImmutableArray<string> FixableDiagnosticIds
		{
			get
			{
				return ImmutableArray.Create("PAIR_0001");
			}
		}

		public override FixAllProvider GetFixAllProvider()
		{
			return WellKnownFixAllProviders.BatchFixer;
		}

		public override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

			context.CancellationToken.ThrowIfCancellationRequested();

			var diagnostic = context.Diagnostics.First();
			var typeNode = root.FindNode(diagnostic.Location.SourceSpan) as TypeDeclarationSyntax;

			var currentBaseList = typeNode.BaseList;
			var newBaseList = typeNode.BaseList.AddTypes(
				SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName("IPairedInternal")));

			var newRoot = root.ReplaceNode(currentBaseList, newBaseList);

			context.RegisterCodeFix(
				CodeAction.Create(
					"Add paired interface",
					_ => Task.FromResult(context.Document.WithSyntaxRoot(newRoot)),
					"Add paired interface"), diagnostic);
		}
	}
}
