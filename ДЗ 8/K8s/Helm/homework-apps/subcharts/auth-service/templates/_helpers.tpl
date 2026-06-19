{{/*
Umbrella base resource prefix (same rules as homework-apps.fullname on parent chart).
*/}}
{{- define "auth-service.releaseBase" -}}
{{- $chartName := default "homework-apps" .Values.global.umbrellaChartName -}}
{{- if contains $chartName .Release.Name -}}
{{- .Release.Name | trunc 63 | trimSuffix "-" -}}
{{- else -}}
{{- printf "%s-%s" .Release.Name $chartName | trunc 63 | trimSuffix "-" -}}
{{- end -}}
{{- end -}}

{{/*
AuthService resource name: <umbrella fullname>-<component>
*/}}
{{- define "auth-service.fullname" -}}
{{- printf "%s-%s" (include "auth-service.releaseBase" .) .Values.name | trunc 63 | trimSuffix "-" }}
{{- end }}

{{- define "auth-service.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" }}
{{- end }}

{{- define "auth-service.labels" -}}
helm.sh/chart: {{ include "auth-service.chart" . }}
app.kubernetes.io/name: {{ .Values.global.kubernetesAppName }}
app.kubernetes.io/instance: {{ .Release.Name }}
app.kubernetes.io/component: {{ .Values.name }}
{{- if .Chart.AppVersion }}
app.kubernetes.io/version: {{ .Chart.AppVersion | quote }}
{{- end }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- end }}

{{- define "auth-service.selectorLabels" -}}
app.kubernetes.io/name: {{ .Values.global.kubernetesAppName }}
app.kubernetes.io/instance: {{ .Release.Name }}
app.kubernetes.io/component: {{ .Values.name }}
{{- end }}

{{- define "auth-service.customerServiceUrl" -}}
{{- $host := printf "%s-%s" (include "auth-service.releaseBase" .) .Values.global.peerCustomerName | trunc 63 | trimSuffix "-" -}}
{{- printf "http://%s.%s.svc.%s" $host .Release.Namespace (default "cluster.local" .Values.global.clusterDomain) -}}
{{- end }}

{{- define "auth-service.billingServiceUrl" -}}
{{- $host := printf "%s-%s" (include "auth-service.releaseBase" .) .Values.global.peerBillingName | trunc 63 | trimSuffix "-" -}}
{{- printf "http://%s.%s.svc.%s" $host .Release.Namespace (default "cluster.local" .Values.global.clusterDomain) -}}
{{- end }}

{{- define "auth-service.authServiceUrl" -}}
{{- $host := printf "%s-%s" (include "auth-service.releaseBase" .) .Values.name | trunc 63 | trimSuffix "-" -}}
{{- printf "http://%s.%s.svc.%s" $host .Release.Namespace (default "cluster.local" .Values.global.clusterDomain) -}}
{{- end }}
