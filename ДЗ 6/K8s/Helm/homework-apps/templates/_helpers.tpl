{{/*
Expand the name of the chart.
*/}}
{{- define "homework-apps.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Create a default fully qualified app name.
We truncate at 63 chars because some Kubernetes name fields are limited to this (by the DNS naming spec).
If release name contains chart name it will be used as a full name.
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
Create resource name for a specific component.
*/}}
{{- define "homework-apps.componentFullname" -}}
{{- $root := .root -}}
{{- $component := .component -}}
{{- printf "%s-%s" (include "homework-apps.fullname" $root) $component | trunc 63 | trimSuffix "-" -}}
{{- end }}

{{/*
Common labels for a specific component.
*/}}
{{- define "homework-apps.componentLabels" -}}
{{- $root := .root -}}
{{- $component := .component -}}
helm.sh/chart: {{ include "homework-apps.chart" $root }}
app.kubernetes.io/name: {{ include "homework-apps.name" $root }}
app.kubernetes.io/instance: {{ $root.Release.Name }}
app.kubernetes.io/component: {{ $component }}
{{- if $root.Chart.AppVersion }}
app.kubernetes.io/version: {{ $root.Chart.AppVersion | quote }}
{{- end }}
app.kubernetes.io/managed-by: {{ $root.Release.Service }}
{{- end }}

{{/*
Selector labels for a specific component.
*/}}
{{- define "homework-apps.componentSelectorLabels" -}}
{{- $root := .root -}}
{{- $component := .component -}}
app.kubernetes.io/name: {{ include "homework-apps.name" $root }}
app.kubernetes.io/instance: {{ $root.Release.Name }}
app.kubernetes.io/component: {{ $component }}
{{- end }}

{{/*
AuthService fullname.
*/}}
{{- define "homework-apps.authFullname" -}}
{{- include "homework-apps.componentFullname" (dict "root" . "component" .Values.authService.name) -}}
{{- end }}

{{/*
AuthService labels.
*/}}
{{- define "homework-apps.authLabels" -}}
{{- include "homework-apps.componentLabels" (dict "root" . "component" .Values.authService.name) -}}
{{- end }}

{{/*
AuthService selector labels.
*/}}
{{- define "homework-apps.authSelectorLabels" -}}
{{- include "homework-apps.componentSelectorLabels" (dict "root" . "component" .Values.authService.name) -}}
{{- end }}

{{/*
CustomerService fullname.
*/}}
{{- define "homework-apps.customerFullname" -}}
{{- include "homework-apps.componentFullname" (dict "root" . "component" .Values.customerService.name) -}}
{{- end }}

{{/*
CustomerService labels.
*/}}
{{- define "homework-apps.customerLabels" -}}
{{- include "homework-apps.componentLabels" (dict "root" . "component" .Values.customerService.name) -}}
{{- end }}

{{/*
CustomerService selector labels.
*/}}
{{- define "homework-apps.customerSelectorLabels" -}}
{{- include "homework-apps.componentSelectorLabels" (dict "root" . "component" .Values.customerService.name) -}}
{{- end }}

{{/*
Resolve service fullname by values key: authService/customerService.
*/}}
{{- define "homework-apps.serviceFullnameByKey" -}}
{{- $root := index . 0 -}}
{{- $serviceKey := index . 1 -}}
{{- $serviceCfg := index $root.Values $serviceKey -}}
{{- include "homework-apps.componentFullname" (dict "root" $root "component" $serviceCfg.name) -}}
{{- end }}

{{/*
In-cluster URL for AuthService.
*/}}
{{- define "homework-apps.authServiceUrl" -}}
{{- printf "http://%s.%s.svc.%s" (include "homework-apps.authFullname" .) .Release.Namespace (default "cluster.local" .Values.common.clusterDomain) -}}
{{- end }}

{{/*
In-cluster URL for CustomerService.
*/}}
{{- define "homework-apps.customerServiceUrl" -}}
{{- printf "http://%s.%s.svc.%s" (include "homework-apps.customerFullname" .) .Release.Namespace (default "cluster.local" .Values.common.clusterDomain) -}}
{{- end }}