{{- define "homework-apps.traefikRequestIdMiddlewareName" -}}
{{- printf "%s-request-id" (include "homework-apps.fullname" .) -}}
{{- end }}

{{- define "homework-apps.traefikRateLimitAuthMiddlewareName" -}}
{{- printf "%s-rate-limit-auth" (include "homework-apps.fullname" .) -}}
{{- end }}

{{- define "homework-apps.traefikRequestIdMiddlewareRef" -}}
{{- printf "%s-%s@kubernetescrd" .Release.Namespace (include "homework-apps.traefikRequestIdMiddlewareName" .) -}}
{{- end }}
