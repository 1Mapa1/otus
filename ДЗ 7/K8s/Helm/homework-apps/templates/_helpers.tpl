{{/*
Expand the name of the chart.
*/}}
{{- define "homework-apps.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Create a default fully qualified app name (umbrella release base name).
*/}}
{{- define "homework-apps.fullname" -}}
{{- if .Values.fullnameOverride }}
{{- .Values.fullnameOverride | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- $name := default .Chart.Name .Values.nameOverride }}
{{- if contains $name .Release.Name }}
{{- .Release.Name | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- printf "%s-%s" .Release.Name $name | trunc 63 | trimSuffix "-" }}
{{- end }}
{{- end }}
{{- end }}

{{/*
Create chart name and version as used by the chart label.
*/}}
{{- define "homework-apps.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Common labels
*/}}
{{- define "homework-apps.labels" -}}
helm.sh/chart: {{ include "homework-apps.chart" . }}
{{ include "homework-apps.selectorLabels" . }}
{{- if .Chart.AppVersion }}
app.kubernetes.io/version: {{ .Chart.AppVersion | quote }}
{{- end }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- end }}

{{/*
Selector labels
*/}}
{{- define "homework-apps.selectorLabels" -}}
app.kubernetes.io/name: {{ include "homework-apps.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end }}

{{/*
Resource name: <umbrella fullname>-<component> (must match subchart auth-service.fullname when Release.Name matches umbrella naming).
Subcharts use Release.Name-component; umbrella fullname equals Release.Name when release name contains chart name.
*/}}
{{- define "homework-apps.componentFullname" -}}
{{- $root := .root -}}
{{- $component := .component -}}
{{- printf "%s-%s" (include "homework-apps.fullname" $root) $component | trunc 63 | trimSuffix "-" -}}
{{- end }}

{{/*
Resolve K8s Service name for ingress by values key authService / customerService.
*/}}
{{- define "homework-apps.serviceFullnameByKey" -}}
{{- $root := index . 0 -}}
{{- $serviceKey := index . 1 -}}
{{- $serviceCfg := index $root.Values $serviceKey -}}
{{- include "homework-apps.componentFullname" (dict "root" $root "component" $serviceCfg.name) -}}
{{- end }}
