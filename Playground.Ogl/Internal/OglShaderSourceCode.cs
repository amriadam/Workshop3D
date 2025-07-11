namespace Playground.Ogl.Internal;

internal static class OglShaderSourceCode
{

    public static string RenderVertexShader =>
        @"
            #version 450 core

            layout (location = 0) uniform mat4 uModelMatrix;
            layout (location = 1) uniform mat4 uViewMatrix;
            layout (location = 2) uniform mat4 uProjectionMatrix;
            layout (location = 3) uniform mat4 uNormalMatrix;
            
            layout (location = 0) in vec3  aVertexPosition;
            layout (location = 1) in vec3  aVertexColor;
            layout (location = 2) in vec3  aVertexNormal;

            flat   out vec4 fragColor;
            smooth out vec3 fragNormal;

            void main()
            {
                vec4 vertexPosition = vec4(aVertexPosition, 1.0);

                gl_Position  = uProjectionMatrix * uViewMatrix * uModelMatrix * vertexPosition;
                fragColor    = vec4(aVertexColor, 1.0);
                fragNormal   = (uNormalMatrix * vec4(aVertexNormal, 0.0)).xyz;
            }
        ";

    public static string RenderFragmentShader => 
        @"
            #version 450 core

            layout (location = 4) uniform int  uLightMode;
            layout (location = 5) uniform vec3 uLightDirection;

            out vec4 FragColor;

            flat   in vec4 fragColor;
            smooth in vec3 fragNormal;

            void main()
            {
                vec4 color = fragColor;

                if (uLightMode == 1)
                {
                    vec3 normal = normalize(fragNormal);

                    float diffuse = clamp(dot(normal, -uLightDirection), 0.0, 1.0);
                    
                    color = vec4(diffuse * color.rgb, color.a);
                }

                FragColor = color;
            }
        ";

    public static string PickingVertexShader =>
        @"
            #version 450 core

            layout (location = 0) uniform mat4 uModelMatrix;
            layout (location = 1) uniform mat4 uViewMatrix;
            layout (location = 2) uniform mat4 uProjectionMatrix;
            
            layout (location = 0) in vec3  aVertexPosition;
            layout (location = 3) in ivec2 aName;

            flat out ivec2 fragColor;

            void main()
            {
                vec4 vertexPosition = vec4(aVertexPosition, 1.0);

                fragColor = aName;

                gl_Position = uProjectionMatrix * uViewMatrix * uModelMatrix * vertexPosition;
            }
        ";

    public static string PickingFragmentShader =>
        @"
            #version 450 core
            
            out ivec4 FragColor;            

            flat in ivec2 fragColor;

            void main()
            {
                FragColor = ivec4(fragColor.r, fragColor.g, 0, 0);
            }
        ";
}
