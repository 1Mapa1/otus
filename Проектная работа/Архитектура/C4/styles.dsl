styles {
    element "Person" {
        shape Person
        background "#0B4884"
        color "#FFFFFF"
    }

    element "Software System" {
        background "#1168BD"
        color "#FFFFFF"
    }

    element "OnlineStore" {
        background "#08427B"
        color "#FFFFFF"
    }

    element "Microservice" {
        background "#438DD5"
        color "#FFFFFF"
    }

    element "Frontend" {
        shape WebBrowser
        background "#438DD5"
        color "#FFFFFF"
    }

    element "Gateway" {
        shape Hexagon
        background "#5B9BD5"
        color "#FFFFFF"
    }

    element "Database" {
        shape Cylinder
        background "#999999"
        color "#FFFFFF"
    }

    element "Cache" {
        shape Cylinder
        background "#D97B29"
        color "#FFFFFF"
    }

    element "MessageBroker" {
        shape Pipe
        background "#875A9E"
        color "#FFFFFF"
    }

    element "Planned" {
        background "#7F8C8D"
        color "#FFFFFF"
    }

    element "OrderComponent" {
		background "#85BBF0"
		color "#000000"
	}

	element "CatalogComponent" {
		background "#85BBF0"
		color "#000000"
	}
	
	element "WarehouseComponent" {
		background "#85BBF0"
		color "#000000"
	}

    relationship "Async" {
        style Dashed
        color "#875A9E"
    }

    relationship "InternalHttp" {
        color "#2E75B6"
    }
}