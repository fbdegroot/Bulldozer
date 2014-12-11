var window = this;
var location = window.location = {
	port: 0,
	href: ''
};

var document = {
	_dummyElement: {
		childNodes: [],
		appendChild: function () { },
		style: {}
	},
	getElementsByTagName: function () { return []; },
	getElementById: function () { return this._dummyElement; },
	createElement: function () { return this._dummyElement; },
	createTextNode: function () { return this._dummyElement; }
};