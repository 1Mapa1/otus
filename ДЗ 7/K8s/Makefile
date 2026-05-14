# Установка стенда ДЗ 7 (Helm + kubectl).
# Запускайте из каталога K8s:  cd "…/ДЗ 7/K8s" && make help
# Нужны: kubectl, helm 3, GNU Make и POSIX shell (WSL, Git Bash, Linux, macOS).

NS           ?= homework
INGRESS_NS   ?= m
APPS_RELEASE ?= homework-apps
CHART_APPS   := Helm/homework-apps

.PHONY: help
help:
	@echo "ДЗ 7 / K8s — типовые цели (namespace приложений: NS=$(NS), ingress: INGRESS_NS=$(INGRESS_NS))."
	@echo ""
	@echo "  make repos            helm repo add (bitnami, ingress-nginx, kafka-ui) + helm repo update"
	@echo "  make ingress-ns       kubectl: namespace для ingress ($(INGRESS_NS))"
	@echo "  make ingress          ingress-nginx (после ingress-ns и repos)"
	@echo "  make homework-ns      kubectl: namespace для Postgres/Kafka/apps ($(NS))"
	@echo "  make postgres         Bitnami PostgreSQL"
	@echo "  make kafka            Kafka (OCI chart)"
	@echo "  make kafka-ui         Kafka UI (опционально)"
	@echo "  make apps             umbrella chart $(APPS_RELEASE) (Helm dependency-update)"
	@echo "  make install          repos → ingress-ns → ingress → homework-ns → postgres → kafka → apps"
	@echo "  make uninstall        снять релизы: apps → kafka-ui → kafka → postgres → ingress (namespaces не трогаем)"
	@echo "  make purge-ns         после uninstall: kubectl delete namespace $(NS) (все объекты в NS)"
	@echo "  make status           kubectl get pods,ingress -n $(NS)"
	@echo ""
	@echo "Переопределение: make install NS=my-ns INGRESS_NS=m"

.PHONY: repos
repos:
	helm repo add ingress-nginx https://kubernetes.github.io/ingress-nginx || true
	helm repo add bitnami https://charts.bitnami.com/bitnami || true
	helm repo add kafka-ui https://provectus.github.io/kafka-ui-charts || true
	helm repo update

.PHONY: ingress-ns
ingress-ns:
	@kubectl get namespace $(INGRESS_NS) >/dev/null 2>&1 || kubectl create namespace $(INGRESS_NS)

.PHONY: ingress
ingress: repos ingress-ns
	helm upgrade --install nginx ingress-nginx/ingress-nginx \
		--namespace $(INGRESS_NS) \
		-f Helm/nginx-ingress.yaml

.PHONY: homework-ns
homework-ns:
	@kubectl get namespace $(NS) >/dev/null 2>&1 || kubectl create namespace $(NS)

.PHONY: postgres
postgres: repos homework-ns
	helm upgrade --install postgres bitnami/postgresql \
		-n $(NS) \
		-f Helm/postgres-values.yaml

.PHONY: kafka
kafka: repos homework-ns
	helm upgrade --install kafka oci://registry-1.docker.io/bitnamicharts/kafka \
		-n $(NS) \
		-f Helm/kafka-values.yaml \
		--wait=false

.PHONY: kafka-ui
kafka-ui: repos homework-ns
	helm upgrade --install kafka-ui kafka-ui/kafka-ui \
		-n $(NS) \
		-f Helm/kafka-ui-values.yaml

.PHONY: apps
apps: repos homework-ns
	helm upgrade --install $(APPS_RELEASE) $(CHART_APPS) \
		-n $(NS) \
		--create-namespace \
		--dependency-update

.PHONY: install
install: repos ingress-ns ingress homework-ns postgres kafka apps

.PHONY: uninstall-apps
uninstall-apps:
	helm uninstall $(APPS_RELEASE) -n $(NS) --ignore-not-found

.PHONY: uninstall-kafka-ui
uninstall-kafka-ui:
	helm uninstall kafka-ui -n $(NS) --ignore-not-found

.PHONY: uninstall-kafka
uninstall-kafka:
	helm uninstall kafka -n $(NS) --ignore-not-found

.PHONY: uninstall-postgres
uninstall-postgres:
	helm uninstall postgres -n $(NS) --ignore-not-found

.PHONY: uninstall-ingress
uninstall-ingress:
	helm uninstall nginx -n $(INGRESS_NS) --ignore-not-found

.PHONY: uninstall
uninstall: uninstall-apps uninstall-kafka-ui uninstall-kafka uninstall-postgres uninstall-ingress

.PHONY: purge-ns
purge-ns:
	kubectl delete namespace $(NS) --ignore-not-found

.PHONY: status
status:
	kubectl get pods -n $(NS)
	kubectl get ingress -n $(NS)
