{{- define "catalog-service.releaseBase" -}}
{{- $chartName := default "homework-apps" .Values.global.umbrellaChartName -}}
{{- if contains $chartName .Release.Name -}}
{{- .Release.Name | trunc 63 | trimSuffix "-" -}}
{{- else -}}
{{- printf "%s-%s" .Release.Name $chartName | trunc 63 | trimSuffix "-" -}}
{{- end -}}
{{- end }}

{{- define "catalog-service.fullname" -}}
{{- printf "%s-%s" (include "catalog-service.releaseBase" .) .Values.name | trunc 63 | trimSuffix "-" }}
{{- end }}

{{- define "catalog-service.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" }}
{{- end }}

{{- define "catalog-service.labels" -}}
helm.sh/chart: {{ include "catalog-service.chart" . }}
app.kubernetes.io/name: {{ .Values.global.kubernetesAppName }}
app.kubernetes.io/instance: {{ .Release.Name }}
app.kubernetes.io/component: {{ .Values.name }}
{{- if .Chart.AppVersion }}
app.kubernetes.io/version: {{ .Chart.AppVersion | quote }}
{{- end }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- end }}

{{- define "catalog-service.selectorLabels" -}}
app.kubernetes.io/name: {{ .Values.global.kubernetesAppName }}
app.kubernetes.io/instance: {{ .Release.Name }}
app.kubernetes.io/component: {{ .Values.name }}
{{- end }}

{{- define "catalog-service.authServiceUrl" -}}
{{- $host := printf "%s-%s" (include "catalog-service.releaseBase" .) .Values.global.peerAuthName | trunc 63 | trimSuffix "-" -}}
{{- printf "http://%s.%s.svc.%s" $host .Release.Namespace (default "cluster.local" .Values.global.clusterDomain) -}}
{{- end }}

{{- define "catalog-service.kafkaBootstrapServersDefault" -}}
{{- $kafkaRelease := .Values.global.kafkaClusterReleaseName | default "kafka" -}}
{{- printf "%s-controller-headless.%s.svc.%s:9092" $kafkaRelease .Release.Namespace (default "cluster.local" .Values.global.clusterDomain) -}}
{{- end }}

{{- define "catalog-service.redisConfigurationDefault" -}}
{{- $host := .Values.global.redisHost | default "redis-master" -}}
{{- $port := .Values.global.redisPort | default "6379" -}}
{{- printf "%s:%s" $host $port -}}
{{- end }}
