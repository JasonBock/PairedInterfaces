using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace PairedInterfaces.Analyzers
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed class EnsurePairedInterfacesExistAnalyzer 
		: DiagnosticAnalyzer
	{
		private static DiagnosticDescriptor descriptor =
			new DiagnosticDescriptor(
				"PAIR_0001",
				"Ensure interfaces are paired",
				"If you implement IPaired, you must implement IPairedInternal",
				"Usage", DiagnosticSeverity.Error, true);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
		{
			get
			{
				return ImmutableArray.Create(EnsurePairedInterfacesExistAnalyzer.descriptor);
			}
		}

		public override void Initialize(AnalysisContext context)
		{
			context.RegisterSymbolAction(
				EnsurePairedInterfacesExistAnalyzer.AnalyzeSymbol, SymbolKind.NamedType);
		}

		private static void AnalyzeSymbol(SymbolAnalysisContext context)
		{
			var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;
			var paired = namedTypeSymbol.AllInterfaces.SingleOrDefault(_ => _.Name == "IPaired");
			var pairedInternal = namedTypeSymbol.AllInterfaces.SingleOrDefault(_ => _.Name == "IPairedInternal");

			if(paired != null && pairedInternal == null)
			{
				context.ReportDiagnostic(Diagnostic.Create(
					EnsurePairedInterfacesExistAnalyzer.descriptor, namedTypeSymbol.Locations[0]));
			}
		}
	}
}
