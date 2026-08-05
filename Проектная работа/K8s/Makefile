# Установка стенда (Helm + kubectl).
# Запускайте из каталога K8s:  cd "…/K8s" && make help
# Нужны: kubectl, helm 3, GNU Make и POSIX shell (WSL, Git Bash, Linux, macOS).

NS              ?= electronics-store
MONITORING_NS   ?= monitoring
INGRESS_NS      ?= m
APPS_RELEASE    ?= homework-apps
CHART_APPS      := Helm/homework-apps
HELM_TIMEOUT    ?= 10m

.PHONY: help preflight
help:
	@echo "K8s — типовые цели (apps: NS=$(NS), monitoring: MONITORING_NS=$(MONITORING_NS), ingress: INGRESS_NS=$(INGRESS_NS))."
	@echo ""
	@echo "  make repos            helm repo add + helm repo update"
	@echo "  make ingress-ns       kubectl: namespace для ingress ($(INGRESS_NS))"
	@echo "  make ingress          Traefik API Gateway"
	@echo "  make apps-ns          kubectl: namespace приложений ($(NS))"
	@echo "  make monitoring-ns    kubectl: namespace monitoring ($(MONITORING_NS))"
	@echo "  make prometheus       kube-prometheus-stack (Grafana + Prometheus)"
	@echo "  make logging          EFK: Elasticsearch + Kibana + Filebeat ($(MONITORING_NS))"
	@echo "  make postgres         PostgreSQL (7 MS, OCI chart)"
	@echo "  make postgres-catalog PostgreSQL для Catalog (OCI chart)"
	@echo "  make redis            Redis (OCI chart)"
	@echo "  make kafka            Kafka (OCI chart)"
	@echo "  make kafka-ui         Kafka UI"
	@echo "  make apps             umbrella chart $(APPS_RELEASE)"
	@echo "  make install-demo     Auth + Customer + Grafana + ELK (без Kafka/Redis)"
	@echo "  make install          основной стенд для API/Postman (без monitoring/logging)"
	@echo "  make install-all      полный стенд: install + Kafka UI + monitoring + logging"
	@echo "  make uninstall        снять основной стенд (namespaces не трогаем)"
	@echo "  make uninstall-all    снять основной и дополнительный стенд"
	@echo "  make purge-ns         удалить namespace $(NS)"
	@echo "  make purge-monitoring-ns  удалить namespace $(MONITORING_NS)"
	@echo "  make status           pods/ingress в $(NS)"
	@echo "  make status-all       status + pods в $(MONITORING_NS)"
	@echo ""
	@echo "Переопределение: make install NS=my-ns MONITORING_NS=monitoring"

preflight:
	@command -v kubectl >/dev/null 2>&1 || { echo "ERROR: kubectl не найден в PATH"; exit 1; }
	@command -v helm >/dev/null 2>&1 || { echo "ERROR: helm не найден в PATH"; exit 1; }
	@kubectl cluster-info >/dev/null 2>&1 || { echo "ERROR: Kubernetes недоступен. Запустите minikube и проверьте kubectl config current-context"; exit 1; }
	@echo "Kubernetes context: $$(kubectl config current-context)"

.PHONY: repos
repos:
	helm repo add traefik https://traefik.github.io/charts || true
	helm repo add kafka-ui https://provectus.github.io/kafka-ui-charts || true
	helm repo add prometheus-community https://prometheus-community.github.io/helm-charts || true
	helm repo add elastic https://helm.elastic.co || true
	helm repo update traefik kafka-ui prometheus-community elastic || true

.PHONY: ingress-ns
ingress-ns:
	@kubectl get namespace $(INGRESS_NS) >/dev/null 2>&1 || kubectl create namespace $(INGRESS_NS)

.PHONY: ingress
ingress: preflight repos ingress-ns
	helm upgrade --install traefik traefik/traefik \
		--namespace $(INGRESS_NS) \
		-f Helm/traefik-values.yaml \
		--wait --timeout $(HELM_TIMEOUT)

.PHONY: apps-ns
apps-ns:
	@kubectl get namespace $(NS) >/dev/null 2>&1 || kubectl create namespace $(NS)

.PHONY: monitoring-ns
monitoring-ns:
	@kubectl get namespace $(MONITORING_NS) >/dev/null 2>&1 || kubectl create namespace $(MONITORING_NS)

.PHONY: prometheus
prometheus: preflight repos monitoring-ns
	helm upgrade --install monitoring prometheus-community/kube-prometheus-stack \
		--namespace $(MONITORING_NS) \
		-f Helm/prometheus-values.yaml \
		--wait --timeout $(HELM_TIMEOUT)
	helm upgrade --install electronics-dashboard Helm/grafana-dashboard \
		--namespace $(MONITORING_NS)

.PHONY: elasticsearch
elasticsearch: preflight repos monitoring-ns
	helm upgrade --install elasticsearch elastic/elasticsearch \
		--namespace $(MONITORING_NS) \
		-f Helm/elasticsearch-values.yaml

.PHONY: kibana
kibana: preflight repos monitoring-ns
	helm upgrade --install kibana Helm/kibana \
		--namespace $(MONITORING_NS) \
		-f Helm/kibana-values.yaml

.PHONY: filebeat
filebeat: preflight repos monitoring-ns
	helm upgrade --install filebeat Helm/filebeat \
		--namespace $(MONITORING_NS) \
		-f Helm/filebeat-values.yaml

.PHONY: logging
logging: elasticsearch kibana filebeat

.PHONY: homework-ns
homework-ns: apps-ns

.PHONY: postgres
postgres: preflight apps-ns
	helm upgrade --install postgres oci://registry-1.docker.io/bitnamicharts/postgresql \
		-n $(NS) \
		-f Helm/postgres-values.yaml \
		--wait --timeout $(HELM_TIMEOUT)

.PHONY: postgres-catalog
postgres-catalog: preflight apps-ns
	helm upgrade --install postgres-catalog oci://registry-1.docker.io/bitnamicharts/postgresql \
		-n $(NS) \
		-f Helm/postgres-catalog-values.yaml \
		--wait --timeout $(HELM_TIMEOUT)

.PHONY: redis
redis: preflight apps-ns
	helm upgrade --install redis oci://registry-1.docker.io/bitnamicharts/redis \
		-n $(NS) \
		-f Helm/redis-values.yaml \
		--wait --timeout $(HELM_TIMEOUT)

.PHONY: kafka
kafka: preflight apps-ns
	helm upgrade --install kafka oci://registry-1.docker.io/bitnamicharts/kafka \
		-n $(NS) \
		-f Helm/kafka-values.yaml \
		--atomic --timeout $(HELM_TIMEOUT)

.PHONY: kafka-ui
kafka-ui: preflight repos apps-ns
	helm upgrade --install kafka-ui kafka-ui/kafka-ui \
		-n $(NS) \
		-f Helm/kafka-ui-values.yaml

.PHONY: apps
apps: preflight apps-ns
	helm upgrade --install $(APPS_RELEASE) $(CHART_APPS) \
		-n $(NS) \
		--create-namespace \
		--dependency-update

.PHONY: apps-demo
apps-demo: preflight apps-ns
	helm upgrade --install $(APPS_RELEASE) $(CHART_APPS) \
		-n $(NS) \
		--create-namespace \
		--dependency-update \
		-f $(CHART_APPS)/demo-values.yaml

.PHONY: demo-prune
demo-prune:
	helm uninstall kafka -n $(NS) --ignore-not-found
	helm uninstall redis -n $(NS) --ignore-not-found
	helm uninstall postgres-catalog -n $(NS) --ignore-not-found

.PHONY: install
install: preflight repos ingress apps-ns postgres postgres-catalog redis kafka apps

.PHONY: install-demo
install-demo: preflight repos ingress apps-ns demo-prune postgres apps-demo prometheus logging

.PHONY: install-all
install-all: install kafka-ui prometheus logging

.PHONY: uninstall-apps
uninstall-apps:
	helm uninstall $(APPS_RELEASE) -n $(NS) --ignore-not-found

.PHONY: uninstall-prometheus
uninstall-prometheus:
	helm uninstall electronics-dashboard -n $(MONITORING_NS) --ignore-not-found
	helm uninstall monitoring -n $(MONITORING_NS) --ignore-not-found

.PHONY: uninstall-logging
uninstall-logging:
	helm uninstall filebeat -n $(MONITORING_NS) --ignore-not-found
	helm uninstall kibana -n $(MONITORING_NS) --ignore-not-found
	helm uninstall elasticsearch -n $(MONITORING_NS) --ignore-not-found

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

.PHONY: uninstall-broken-default
uninstall-broken-default:
	helm uninstall traefik -n default --ignore-not-found
	helm uninstall monitoring -n default --ignore-not-found

.PHONY: uninstall
uninstall: uninstall-apps uninstall-kafka uninstall-redis uninstall-postgres-catalog uninstall-postgres uninstall-ingress

.PHONY: uninstall-all
uninstall-all: uninstall-apps uninstall-kafka-ui uninstall-kafka uninstall-redis uninstall-postgres-catalog uninstall-postgres uninstall-logging uninstall-prometheus uninstall-ingress

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

.PHONY: status-all
status-all: status
	kubectl get pods -n $(MONITORING_NS)
