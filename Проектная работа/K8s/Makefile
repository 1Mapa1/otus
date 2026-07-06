# Установка стенда (Helm + kubectl).
# Запускайте из каталога K8s:  cd "…/K8s" && make help
# Нужны: kubectl, helm 3, GNU Make и POSIX shell (WSL, Git Bash, Linux, macOS).

NS              ?= electronics-store
MONITORING_NS   ?= monitoring
INGRESS_NS      ?= m
APPS_RELEASE    ?= homework-apps
CHART_APPS      := Helm/homework-apps

.PHONY: help
help:
	@echo "K8s — типовые цели (apps: NS=$(NS), monitoring: MONITORING_NS=$(MONITORING_NS), ingress: INGRESS_NS=$(INGRESS_NS))."
	@echo ""
	@echo "  make repos            helm repo add + helm repo update"
	@echo "  make ingress-ns       kubectl: namespace для ingress ($(INGRESS_NS))"
	@echo "  make ingress          Traefik API Gateway"
	@echo "  make apps-ns          kubectl: namespace приложений ($(NS))"
	@echo "  make monitoring-ns    kubectl: namespace monitoring ($(MONITORING_NS))"
	@echo "  make prometheus       kube-prometheus-stack (Grafana + Prometheus)"
	@echo "  make postgres         Bitnami PostgreSQL (7 MS)"
	@echo "  make postgres-catalog Bitnami PostgreSQL для Catalog"
	@echo "  make redis            Bitnami Redis"
	@echo "  make kafka            Kafka (OCI chart)"
	@echo "  make kafka-ui         Kafka UI"
	@echo "  make apps             umbrella chart $(APPS_RELEASE)"
	@echo "  make install          полный стенд (включая prometheus)"
	@echo "  make uninstall        снять релизы (namespaces не трогаем)"
	@echo "  make purge-ns         удалить namespace $(NS)"
	@echo "  make purge-monitoring-ns  удалить namespace $(MONITORING_NS)"
	@echo "  make status           pods/ingress в $(NS)"
	@echo ""
	@echo "Переопределение: make install NS=my-ns MONITORING_NS=monitoring"

.PHONY: repos
repos:
	helm repo add traefik https://traefik.github.io/charts || true
	helm repo add bitnami https://charts.bitnami.com/bitnami || true
	helm repo add kafka-ui https://provectus.github.io/kafka-ui-charts || true
	helm repo add prometheus-community https://prometheus-community.github.io/helm-charts || true
	helm repo update

.PHONY: ingress-ns
ingress-ns:
	@kubectl get namespace $(INGRESS_NS) >/dev/null 2>&1 || kubectl create namespace $(INGRESS_NS)

.PHONY: ingress
ingress: repos ingress-ns
	helm upgrade --install traefik traefik/traefik \
		--namespace $(INGRESS_NS) \
		-f Helm/traefik-values.yaml

.PHONY: apps-ns
apps-ns:
	@kubectl get namespace $(NS) >/dev/null 2>&1 || kubectl create namespace $(NS)

.PHONY: monitoring-ns
monitoring-ns:
	@kubectl get namespace $(MONITORING_NS) >/dev/null 2>&1 || kubectl create namespace $(MONITORING_NS)

.PHONY: prometheus
prometheus: repos monitoring-ns
	helm upgrade --install monitoring prometheus-community/kube-prometheus-stack \
		--namespace $(MONITORING_NS) \
		-f Helm/prometheus-values.yaml

.PHONY: homework-ns
homework-ns: apps-ns

.PHONY: postgres
postgres: repos apps-ns
	helm upgrade --install postgres bitnami/postgresql \
		-n $(NS) \
		-f Helm/postgres-values.yaml

.PHONY: postgres-catalog
postgres-catalog: repos apps-ns
	helm upgrade --install postgres-catalog bitnami/postgresql \
		-n $(NS) \
		-f Helm/postgres-catalog-values.yaml

.PHONY: redis
redis: repos apps-ns
	helm upgrade --install redis bitnami/redis \
		-n $(NS) \
		-f Helm/redis-values.yaml

.PHONY: kafka
kafka: repos apps-ns
	helm upgrade --install kafka oci://registry-1.docker.io/bitnamicharts/kafka \
		-n $(NS) \
		-f Helm/kafka-values.yaml \
		--wait=false

.PHONY: kafka-ui
kafka-ui: repos apps-ns
	helm upgrade --install kafka-ui kafka-ui/kafka-ui \
		-n $(NS) \
		-f Helm/kafka-ui-values.yaml

.PHONY: apps
apps: repos apps-ns
	helm upgrade --install $(APPS_RELEASE) $(CHART_APPS) \
		-n $(NS) \
		--create-namespace \
		--dependency-update

.PHONY: install
install: repos ingress-ns ingress apps-ns monitoring-ns prometheus postgres postgres-catalog redis kafka kafka-ui apps

.PHONY: uninstall-apps
uninstall-apps:
	helm uninstall $(APPS_RELEASE) -n $(NS) --ignore-not-found

.PHONY: uninstall-prometheus
uninstall-prometheus:
	helm uninstall monitoring -n $(MONITORING_NS) --ignore-not-found

.PHONY: uninstall-kafka-ui
uninstall-kafka-ui:
	helm uninstall kafka-ui -n $(NS) --ignore-not-found

.PHONY: uninstall-kafka
uninstall-kafka:
	helm uninstall kafka -n $(NS) --ignore-not-found

.PHONY: uninstall-redis
uninstall-redis:
	helm uninstall redis -n $(NS) --ignore-not-found

.PHONY: uninstall-postgres-catalog
uninstall-postgres-catalog:
	helm uninstall postgres-catalog -n $(NS) --ignore-not-found

.PHONY: uninstall-postgres
uninstall-postgres:
	helm uninstall postgres -n $(NS) --ignore-not-found

.PHONY: uninstall-ingress
uninstall-ingress:
	helm uninstall traefik -n $(INGRESS_NS) --ignore-not-found

.PHONY: uninstall
uninstall: uninstall-apps uninstall-kafka-ui uninstall-kafka uninstall-redis uninstall-postgres-catalog uninstall-postgres uninstall-prometheus uninstall-ingress

.PHONY: purge-ns
purge-ns:
	kubectl delete namespace $(NS) --ignore-not-found

.PHONY: purge-monitoring-ns
purge-monitoring-ns:
	kubectl delete namespace $(MONITORING_NS) --ignore-not-found

.PHONY: status
status:
	kubectl get pods -n $(NS)
	kubectl get ingress -n $(NS)
	kubectl get pods -n $(MONITORING_NS)
