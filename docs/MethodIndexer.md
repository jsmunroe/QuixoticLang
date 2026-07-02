# Generic MethodIndexer<TDelegate> Implementation

## Overview
The `MethodIndexer<TDelegate>` is a **fully generic reflection-based method dispatch system** that automatically builds a lookup table for polymorphic method calls using delegates.

## How It Works

### Key Concept
Instead of manually writing reflection code for each delegate type, `MethodIndexer` **introspects the delegate type itself** to understand what signature to match against.

### Example Usage

```csharp
// Define your delegate signatures
public delegate void StatementAnalyzer(SemanticAnalyzer instance, QxStatement statement);
public delegate QxType ExpressionAnalyzer(SemanticAnalyzer instance, QxExpression expression);

// Create indexers - they automatically find matching methods
var statementIndexer = new MethodIndexer<Action<SemanticAnalyzer, QxStatement>>(
	typeof(SemanticAnalyzer), 
	"Analyze");

var expressionIndexer = new MethodIndexer<Func<SemanticAnalyzer, QxExpression, QxType>>(
	typeof(SemanticAnalyzer), 
	"Analyze");

// Use them for polymorphic dispatch
QxStatement statement = new QxPrintStatement(...);
if (statementIndexer.TryGetDelegate(statement.GetType(), out var handler))
{
	handler(analyzer, statement);  // Calls Analyze(QxPrintStatement)
}
```

## Architecture

### Delegate Introspection
```csharp
public MethodIndexer(Type targetType, string methodName)
{
	// Extract delegate signature information
	var invokeMethod = typeof(TDelegate).GetMethod("Invoke");

	_delegateParameterTypes = invokeMethod.GetParameters()
		.Select(p => p.ParameterType)
		.ToArray();

	_delegateReturnType = invokeMethod.ReturnType;
}
```

**For `Action<SemanticAnalyzer, QxStatement>`:**
- Parameters: `[SemanticAnalyzer, QxStatement]`
- Return: `void`

**For `Func<SemanticAnalyzer, QxExpression, QxType>`:**
- Parameters: `[SemanticAnalyzer, QxExpression, QxType]`
- Return: `QxType`

### Method Matching

The indexer scans the target type for methods that:
1. ✅ Have the specified name ("Analyze")
2. ✅ Have compatible return type
3. ✅ Have compatible parameter signature

### Expression Tree Compilation

For each matching method, creates a compiled delegate:

```csharp
// Example: Analyze(QxPrintStatement statement)
// Becomes: Action<SemanticAnalyzer, QxStatement>

// 1. Create expression parameters
var instance = Expression.Parameter(typeof(SemanticAnalyzer));
var statement = Expression.Parameter(typeof(QxStatement));

// 2. Cast to specific type
var castStatement = Expression.Convert(statement, typeof(QxPrintStatement));

// 3. Build method call
var call = Expression.Call(instance, method, castStatement);

// 4. Compile to delegate
var lambda = Expression.Lambda<TDelegate>(call, instance, statement);
var compiled = lambda.Compile();
```

### Keying Strategy

Methods are indexed by their **first polymorphic parameter** (after instance):
- Instance methods: `method.Parameters[0]` (first parameter)
- Static methods: `method.Parameters[1]` (second parameter, after instance)

```csharp
Dictionary<Type, TDelegate> _methodMap
{
	[typeof(QxPrintStatement)] = (instance, stmt) => instance.Analyze((QxPrintStatement)stmt),
	[typeof(QxNumberLiteralExpression)] = (instance, expr) => instance.Analyze((QxNumberLiteralExpression)expr),
	...
}
```

## Usage in SemanticAnalyzer

```csharp
public class SemanticAnalyzer
{
	// Create indexers for different signatures
	private static readonly MethodIndexer<Action<SemanticAnalyzer, QxStatement>> _statementIndexer = 
		new(typeof(SemanticAnalyzer), "Analyze");

	private static readonly MethodIndexer<Func<SemanticAnalyzer, QxExpression, QxType>> _expressionIndexer = 
		new(typeof(SemanticAnalyzer), "Analyze");

	// Polymorphic dispatch methods
	private void Analyze(QxStatement statement)
	{
		if (_statementIndexer.TryGetDelegate(statement.GetType(), out var handler))
			handler(this, statement);
		else
			throw new NotImplementedException($"No analyzer for {statement.GetType().Name}");
	}

	private QxType Analyze(QxExpression expression)
	{
		if (_expressionIndexer.TryGetDelegate(expression.GetType(), out var handler))
			return handler(this, expression);

		throw new NotImplementedException($"No analyzer for {expression.GetType().Name}");
	}

	// Specific implementations - automatically discovered
	private void Analyze(QxPrintStatement statement) { ... }
	private void Analyze(QxIfStatement statement) { ... }

	private QxType Analyze(QxNumberLiteralExpression expr) => QxType.Number;
	private QxType Analyze(QxBinaryExpression expr) { ... }
}
```

## Benefits

### 1. **Type Safety**
Compile-time checking of delegate signatures

### 2. **Performance**
- One-time reflection during static initialization
- Compiled expression trees (nearly as fast as direct calls)
- No runtime reflection overhead

### 3. **Maintainability**
- Add new analyzers without modifying dispatch code
- Compiler catches signature mismatches
- Clear separation of concerns

### 4. **Reusability**
The same `MethodIndexer<TDelegate>` works for any delegate type

## Pattern Comparison

### Without MethodIndexer (Manual Visitor Pattern)
```csharp
public interface IStatementVisitor
{
	void Visit(QxPrintStatement stmt);
	void Visit(QxIfStatement stmt);
	// Must add method for EVERY statement type
}

// Every statement needs Accept method
public abstract class QxStatement
{
	public abstract void Accept(IStatementVisitor visitor);
}
```

### With MethodIndexer (Automatic Discovery)
```csharp
// Just write methods - no interfaces needed!
private void Analyze(QxPrintStatement statement) { ... }
private void Analyze(QxIfStatement statement) { ... }

// Automatically dispatched
```

## Advanced Features

### Supports Multiple Parameter Types
```csharp
// Could extend to support context parameters
delegate QxType Analyzer(SemanticAnalyzer self, QxExpression expr, AnalysisContext context);
```

### Handles Inheritance
```csharp
// If you have:
void Analyze(QxBinaryExpression expr) { ... }

// It will match:
QxAddExpression : QxBinaryExpression
QxMultiplyExpression : QxBinaryExpression
// etc.
```

## Performance Characteristics

- **Initialization**: O(M) where M = number of methods in target type
- **Lookup**: O(1) dictionary lookup
- **Invocation**: Nearly identical to direct method call

## Similar Patterns in Other Languages

This pattern is similar to:
- **C#**: `dynamic` keyword (but faster and type-safe)
- **Java**: Method handles / MethodType
- **JavaScript**: Prototype-based dispatch
- **Rust**: Trait objects with vtables

## Future Enhancements

1. **Caching across types**: Share indexers for inheritance hierarchies
2. **Fallback handlers**: Default behavior for unmatched types
3. **Async support**: `Func<T, Task<TResult>>` delegates
4. **Multi-dispatch**: Match on multiple parameters

## Conclusion

`MethodIndexer<TDelegate>` provides **visitor pattern benefits without visitor pattern boilerplate**. It's a powerful meta-programming tool that makes polymorphic dispatch elegant and maintainable.
