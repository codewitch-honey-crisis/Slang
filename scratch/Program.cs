using CD;
using Slang;
using System;
using System.CodeDom;
using System.IO;
using System.Linq;
using System.Reflection;

namespace scratch
{
	using CU = CD.CodeDomUtility;
	class Program
	{
		static void Main()
		{
			//RunTemplate();
			//RunBinding();
			RunResolver();
		}
		static void RunResolver()
		{
			// create a resolver
			var res = new CodeDomResolver();
			
			// read the resolver sample into the compile unit
			CodeCompileUnit ccu;
			using (var stm = File.OpenRead(@"..\..\Resolver.cs"))
				ccu = SlangParser.ReadCompileUnitFrom(stm);
			
			// remember to patch it!
			SlangPatcher.Patch(ccu);
			
			// add the compile unit to the resolver
			res.CompileUnits.Add(ccu);

			// prepare the resolver
			// any time you add compile units you'll need
			// to call Refresh()
			res.Refresh();

			// go through all expressions in the
			// graph and try to get their type
			CodeDomVisitor.Visit(ccu, (ctx) => {
				var expr = ctx.Target as CodeExpression;
				if (null != expr) 
				{
					// we want everything except CodeTypeReferenceExpression
					var ctre = expr as CodeTypeReferenceExpression;
					if (null == ctre)
					{
						// get the scope of the expression
						var scope = res.GetScope(expr);
						CodeTypeReference ctr = res.TryGetTypeOfExpression(expr,scope);
						if (null != ctr)
						{
							Console.WriteLine(CU.ToString(expr) + " is type: " + CU.ToString(ctr));
							Console.WriteLine("Scope Dump:");
							Console.WriteLine(scope.ToString());
						}
					}
				}
			});

		}
		static void RunBinding()
		{
			// we'll need the resolver in a bit
			var res = new CodeDomResolver();
			
			// read the binding sample into the compile unit
			CodeCompileUnit ccu;
			using (var stm = File.OpenRead(@"..\..\Binding.cs"))
				ccu = SlangParser.ReadCompileUnitFrom(stm);
			
			// add the compile unit to the resolver
			res.CompileUnits.Add(ccu);

			// prepare the resolver
			res.Refresh();

			// get the first class available
			var tdecl = ccu.Namespaces[1].Types[0];
			// capture the scope at the typedecl level
			var scope = res.GetScope(tdecl);
			// create a new binder with that scope
			var binder = new CodeDomBinder(scope);
			// get the method group for Test(...)
			var methodGroup = binder.GetMethodGroup(tdecl, "Test", BindingFlags.Public | BindingFlags.Instance);
			
			// select the method that can take a string value
			var m =binder.SelectMethod(BindingFlags.Public, methodGroup, new CodeTypeReference[] { new CodeTypeReference(typeof(string)) }, null);
			Console.WriteLine(CU.ToString((CodeMemberMethod)m));
			
			// select the method that can take a short value
			// (closest match accepts int)
			m = binder.SelectMethod(BindingFlags.Public, methodGroup, new CodeTypeReference[] { new CodeTypeReference(typeof(short)) }, null);
			Console.WriteLine(CU.ToString((CodeMemberMethod)m));


		}
		static void RunTemplate()
		{
			// compute the primes. algorithm borrowed
			// from SLax at https://stackoverflow.com/questions/1510124/program-to-find-prime-numbers
			var primesMax = 100;
			var primesArr = Enumerable.Range(0, (int)Math.Floor(2.52 * Math.Sqrt(primesMax) / Math.Log(primesMax))).Aggregate(
				Enumerable.Range(2, primesMax - 1).ToList(),
				(result, index) =>
				{
					var bp = result[index]; var sqr = bp * bp;
					result.RemoveAll(i => i >= sqr && i % bp == 0);
					return result;
				}
			).ToArray();

			// read the template into the compile unit
			CodeCompileUnit ccu;
			using (var stm = File.OpenRead(@"..\..\Template.cs"))
				ccu=SlangParser.ReadCompileUnitFrom(stm);
			// find the target namespace and change it
			var ns = CU.GetByName("T_NAMESPACE", ccu.Namespaces);
			ns.Name = "TestNS";
			// find the target class
			var type= CU.GetByName("T_TYPE", ns.Types);
			// change the name
			type.Name = "TestPrimes";
			// get the Primes field:
			var primes = CU.GetByName("Primes", type.Members) as CodeMemberField;
			// change the init expression to the primes array
			primes.InitExpression = CU.Literal(primesArr);
			// now write the result out
			Console.WriteLine(CU.ToString(ccu));
		}
	}
}
