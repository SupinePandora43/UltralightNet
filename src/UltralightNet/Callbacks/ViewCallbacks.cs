namespace UltralightNet.Callbacks;

// public delegate void ChangeTitleCallback(string title);
// public delegate void ChangeURLCallback(string url);
// public delegate void ChangeTooltipCallback(string tooltip);
// public delegate void ChangeCursorCallback(ULCursor cursor);
public delegate void AddConsoleMessageCallback(
	ULMessageSource source,
	ULMessageLevel level,
	string message,
	uint lineNumber,
	uint columnNumber,
	string sourceId
);
public delegate View? CreateChildViewCallback(
	string openerUrl,
	string targetUrl,
	bool isPopup,
	ULIntRect popupRect
);
public delegate View? CreateInspectorViewCallback(
	bool isLocal,
	string inspectedUrl
);
public delegate void BeginLoadingCallback(
	ulong frameId,
	bool isMainFrame,
	string url
);
public delegate void FinishLoadingCallback(
	ulong frameId,
	bool isMainFrame,
	string url
);
public delegate void FailLoadingCallback(
	ulong frameId,
	bool isMainFrame,
	string url,
	string description,
	string errorDomain,
	int errorCode
);
public delegate void WindowObjectReadyCallback(
	ulong frameId,
	bool isMainFrame,
	string url
);
public delegate void DOMReadyCallback(
	ulong frameId,
	bool isMainFrame,
	string url
);
// public delegate void UpdateHistoryCallback();
