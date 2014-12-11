function Compile(code) {
	var output;

	try {
		(new window.less.Parser()).parse(code, function (error, tree) {
			if (error)
				output = "Error on line " + error.line + ":\n" + error.message;
			else
				output = tree.toCSS({ compress: false });
		});
	}
	catch (error) {
		if (error.line)
			output = "Error on line " + error.line + ":\n" + error.message;
		else
			output = "Could not parse line " + error.callLine + ":\n" + error.callExtract;
	}

	return output;
}