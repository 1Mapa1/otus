workspace "Интернет-магазин электроники" "C4-архитектура проекта" {

    !identifiers hierarchical

    model {
		!include model/platform.dsl
		!include model/platform-relations.dsl
		!include model/order-ms-relations.dsl
		!include model/catalog-ms-relations.dsl
		!include model/warehouse-ms-relations.dsl
	}

	views {
		!include views/overview.dsl
		!include views/order-ms.dsl
		!include views/catalog-ms.dsl
		!include views/warehouse-ms.dsl
		!include styles.dsl
	}
}