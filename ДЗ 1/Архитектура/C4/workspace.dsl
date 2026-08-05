workspace "I'll Have the BLT" "Декомпозиция интернет-заказов сети сэндвич-кафе" {
    !identifiers hierarchical

    model {
        !include model/platform.dsl
        !include model/relations.dsl
    }

    views {
        !include views/containers.dsl
        !include styles.dsl
    }
}

