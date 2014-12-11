function Compile(code, bare, header) {
	var output;

	try {
		output = CoffeeScript.compile(code, { bare: bare, header: header });
	}
	catch (error) {
		output = "Error on line " + error.location.first_line + " on column " + error.location.first_column + ":\n" + error.message;
	}

	return output;
}