# Type System Migration to Common

## Overview
The QuixoticLang type system has been successfully migrated from `Quixotic.Interpret` to `Quixotic.Common` to enable sharing between the Interpreter and the new SemanticAnalyzer.

## Changes Made

### New Files Created in `Quixotic.Common\Types\`
1. **QxType.cs** - Core type class with:
   - Type equality and assignability checking
   - Common base type resolution
   - Type parsing from strings
   - Static instances for built-in types (Number, String, Boolean, Nada, Void, Any)

2. **QxValueType.cs** - Base class for value types
3. **NumberType.cs** - Number type singleton
4. **StringType.cs** - String type singleton
5. **BooleanType.cs** - Boolean type singleton
6. **NadaType.cs** - Nada (null) type singleton
7. **VoidType.cs** - Void type singleton
8. **ArrayType.cs** - Generic array type

### Files Modified in `Quixotic.Interpret`

#### Backward Compatibility Shims
The following files were converted to re-export types from Common:
- `Symbols\Types\QxType.cs` - Now contains extension methods
- `Symbols\Types\QxValueType.cs`
- `Symbols\Types\NumberType.cs`
- `Symbols\Types\StringType.cs`
- `Symbols\Types\BooleanType.cs`
- `Symbols\Types\NadaType.cs`
- `Symbols\Types\VoidType.cs`
- `Symbols\Types\ArrayType.cs`

#### Updated Imports
Added `using Quixotic.Common.Types;` to:
- `Symbols\Values\Instance.cs`
- `Symbols\Values\Value.cs`
- `Symbols\Values\ArrayInstance.cs`
- `Symbols\Values\NumberValue.cs`
- `Symbols\Values\StringValue.cs`
- `Symbols\Values\BooleanValue.cs`
- `Symbols\Values\NadaValue.cs`
- `Symbols\Values\Reference.cs`
- `Symbols\Parameter.cs`
- `Symbols\VariableSymbol.cs`
- `Environment\Scope.cs`
- `Interpreter.cs`

#### Test Files Updated
- `InterpretTests\TestImplementations\TestRuntime.cs`

### SemanticAnalyzer Enhancement

Updated `Quixotic.Analysis\Semantics\SemanticAnalyzer.cs` with:
- **SymbolTable** class implementing:
  - Scoped symbol storage with parent chain
  - `Define(name, type)` - Define new symbols
  - `Resolve(name)` - Lookup symbols with scope chain traversal
  - `TryResolve(name, out type)` - Safe lookup
  - `CreateChild()` - Create nested scopes

## Architecture Benefits

### Single Source of Truth
- Types are defined once in `Common`
- Both Interpreter and SemanticAnalyzer use the same types
- No translation or mapping needed

### Separation of Concerns
```
Quixotic.Common.Types
	↓
	├─→ Quixotic.Analysis (SemanticAnalyzer)
	└─→ Quixotic.Interpret (Interpreter)
```

### Standard Compiler Pipeline
```
Source Code
	↓
Lexer → Tokens
	↓
Parser → AST
	↓
SemanticAnalyzer → Type-checked AST  ⬅️ NEW
	↓
Interpreter → Execution
```

## Type System Features

### Built-in Types
- `QxType.Number` - Numeric values (double)
- `QxType.String` - Text values
- `QxType.Boolean` - True/false values
- `QxType.Nada` - Null/undefined
- `QxType.Void` - Function return void
- `QxType.Any` - Top type for type inference

### Type Operations
- **Assignability**: `type1.IsAssignableFrom(type2)`
- **Common Base**: `QxType.GetCommonBase(types)`
- **Parsing**: `QxType.Parse("number[]")` or `QxType.TryParse(...)`
- **Arrays**: `QxType.Array(elementType)` or `new ArrayType(elementType)`

## Next Steps

### For SemanticAnalyzer Implementation:
1. **Type Checking**
   - Binary operations: `+ - * / && ||`
   - Unary operations: `! - +`
   - Assignment compatibility

2. **Function Analysis**
   - Parameter type validation
   - Return type checking
   - Call site argument matching

3. **Scope Validation**
   - Undefined variable detection
   - Duplicate declaration detection
   - Block scoping for if/do/for

4. **Type Inference**
   - Array element types
   - Expression result types

## Testing Recommendations
- Unit tests for SymbolTable scope chain
- Type assignability tests
- Type parsing tests
- Integration tests: Parser → SemanticAnalyzer → Interpreter

## Migration Status
✅ Type system moved to Common
✅ All Interpret files updated
✅ Test files updated
✅ Build successful
✅ SymbolTable basic implementation complete
⏳ SemanticAnalyzer implementation (TODO)
